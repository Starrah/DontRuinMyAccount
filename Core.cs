using System.Reflection;
using HarmonyLib;
using MAI2.Util;
using Manager;
using Manager.UserDatas;
using MelonLoader;
using Process;
using UnityEngine;

[assembly: MelonInfo(typeof(DontRuinMyAccount.Core), "DontRuinMyAccount", "1.0.0", "Starrah", "https://github.com/Starrah/DontRuinMyAccount")]
[assembly: MelonGame("sega-interactive", "Sinmai")]

namespace DontRuinMyAccount;

[HarmonyPatch]
public class Core : MelonMod
{
    private static uint currentTrackNumber => GameManager.MusicTrackNumber;
    private static bool ignoreScore;
    private static UserScore oldScore;

    public override void OnInitializeMelon()
    {
        LoggerInstance.Msg("Initialized.");
    }

    public override void OnUpdate()
    {
        if (GameManager.IsInGame && GameManager.IsAutoPlay() && !ignoreScore)
        {
            ignoreScore = true;
            LoggerInstance.Msg("Autoplay triggered, will ignore this score.");
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UserData), "UpdateScore")]
    public static bool BeforeUpdateScore(int musicid, int difficulty, uint achive, uint romVersion)
    {
        if (ignoreScore)
        {
            Melon<Core>.Logger.Msg("Prevented update DXRating: trackNo {0}, music {1}:{2}, achievement {3}", currentTrackNumber, musicid, difficulty, achive);
            return false;
        }
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ResultProcess), "OnStart")]
    [HarmonyPriority(HarmonyLib.Priority.High)]
    public static bool BeforeResultProcessStart()
    {
        if (!ignoreScore)
        {
            return true;
        }
        var musicid = GameManager.SelectMusicID[0];
        var difficulty = GameManager.SelectDifficultyID[0];
        var userData = Singleton<UserDataManager>.Instance.GetUserData(0);
        // deepcopy
        oldScore = JsonUtility.FromJson<UserScore>(JsonUtility.ToJson(userData.ScoreDic[difficulty].GetValueSafe(musicid)));
        Melon<Core>.Logger.Msg("Stored old score: trackNo {0}, music {1}:{2}, achievement {3}", currentTrackNumber, musicid, difficulty, oldScore?.achivement);
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ResultProcess), "OnStart")]
    [HarmonyPriority(HarmonyLib.Priority.High)]
    public static void AfterResultProcessStart()
    {
        if (!ignoreScore)
        {
            return;
        }
        ignoreScore = false;
        var musicid = GameManager.SelectMusicID[0];
        var difficulty = GameManager.SelectDifficultyID[0];
        // current music playlog
        var score = Singleton<GamePlayManager>.Instance.GetGameScore(0, (int)currentTrackNumber - 1);
        // score.Achivement = 0; // Private setter, so reflection is essential
        typeof(GameScoreList).GetProperty("Achivement", BindingFlags.Public | BindingFlags.Instance)?.GetSetMethod(true)?.Invoke(score, new object[]
        {
            new Decimal(0)
        });
        // user's all scores
        var userData = Singleton<UserDataManager>.Instance.GetUserData(0);
        var userScoreDict = userData.ScoreDic[difficulty];
        if (oldScore != null)
        {
            userScoreDict[musicid] = oldScore;
        }
        else
        {
            userScoreDict.Remove(musicid);
        }
        Melon<Core>.Logger.Msg("Reset scores: trackNo {0}, music {1}:{2}, set current music playlog to 0.0000%, and userScoreDict[{1}:{2}] to {3}", currentTrackNumber,
            musicid, difficulty, oldScore?.achivement);
    }
}