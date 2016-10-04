using System;
using Spate;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class AssetAPI
{
    public static void LoadActorMesh(string name)
    {
        AssetManager.Load(string.Format("actor/{0}.ab", name));
    }

    public static GameObject GetActorMesh(string name)
    {
        return AssetManager.GetObject<GameObject>(string.Format("actor/{0}.ab", name));
    }

    public static void LoadBGM(string name)
    {
        AssetManager.Load(string.Format("bgm/{0}.ab", name));
    }

    public static AudioClip GetBGM(string name)
    {
        return AssetManager.GetObject<AudioClip>(string.Format("bgm/{0}.ab", name));
    }

    public static void LoadVoice(string name)
    {
        AssetManager.Load(string.Format("voice/{0}.ab", name));
    }

    public static AudioClip GetVoice(string name)
    {
        return AssetManager.GetObject<AudioClip>(string.Format("voice/{0}.ab", name));
    }

    public static void LoadWeapon(string name)
    {
        AssetManager.Load(string.Format("weapon/{0}.ab", name));
    }

    public static GameObject GetWeapon(string name)
    {
        return AssetManager.GetObject<GameObject>(string.Format("weapon/{0}.ab", name));
    }

    //public static void LoadCategory(string category)
    //{
    //    List<string> list = AssetManager.GetCategory(category);
    //    foreach (string entry in list)
    //    {
    //        AssetManager.Load(entry, false);
    //    }
    //}

    //public static void LoadActors(IList<ActorData> list)
    //{
    //    if (list != null)
    //    {
    //        for (int i = 0; i != list.Count; i++)
    //        {
    //            LoadActor(list[i]);
    //        }
    //    }
    //}

    //public static void LoadActor(ActorData data)
    //{
    //    if (data != null)
    //    {
    //        LoadActorMesh(data.actorMesh);
    //        if (StringUtil.CheckString(data.actorWeapon))
    //            LoadWeapon(data.actorWeapon);
    //        if (StringUtil.CheckString(data.actorVoice))
    //            LoadVoice(data.actorVoice);
    //    }
    //}

    //public static void LoadActorEffects(ActorData data)
    //{
    //    // 普通技
    //    if (data.normalSkill != null)
    //    {
    //        // 起手特效
    //        LoadEffect(data.normalSkill.effect);
    //        // 武器特效
    //        LoadEffect(data.normalSkill.wpEffect);
    //        // 受体特效
    //        if (data.normalSkill.listEvents != null)
    //        {
    //            for (int i = 0; i < data.normalSkill.listEvents.Count; i++)
    //            {
    //                if (data.normalSkill.listEvents[i].isBullet)
    //                {
    //                    LoadEffect(data.normalSkill.listEvents[i].bullet.res);
    //                    LoadEffect(data.normalSkill.listEvents[i].bullet.deadEffect);
    //                }
    //                LoadEffect(data.normalSkill.listEvents[i].recepterEffect);
    //            }
    //        }
    //    }
    //    // 定时技
    //    if (data.timedSkill != null && data.timedSkill.Length > 0)
    //    {
    //        for (int i = 0; i < data.timedSkill.Length; i++)
    //        {
    //            if (data.timedSkill[i] == null)
    //                continue;
    //            // 起手特效
    //            LoadEffect(data.timedSkill[i].effect);
    //            // 武器特效
    //            LoadEffect(data.timedSkill[i].wpEffect);
    //            // 受体特效
    //            if (data.timedSkill[i].listEvents != null)
    //            {
    //                for (int x = 0; x < data.timedSkill[i].listEvents.Count; x++)
    //                {
    //                    if (data.timedSkill[i].listEvents[x].isBullet)
    //                    {
    //                        LoadEffect(data.timedSkill[i].listEvents[x].bullet.res);
    //                        LoadEffect(data.timedSkill[i].listEvents[x].bullet.deadEffect);
    //                    }
    //                    LoadEffect(data.timedSkill[i].listEvents[x].recepterEffect);
    //                }
    //            }
    //        }
    //    }
    //    // 必杀技
    //    if (data.uniqueSkill != null)
    //    {
    //        // 起手特效
    //        LoadEffect(data.uniqueSkill.effect);
    //        // 武器特效
    //        LoadEffect(data.uniqueSkill.wpEffect);
    //        // 受体特效
    //        if (data.uniqueSkill.listEvents != null)
    //        {
    //            for (int i = 0; i < data.uniqueSkill.listEvents.Count; i++)
    //            {
    //                if (data.uniqueSkill.listEvents[i].isBullet)
    //                {
    //                    LoadEffect(data.uniqueSkill.listEvents[i].bullet.res);
    //                    LoadEffect(data.uniqueSkill.listEvents[i].bullet.deadEffect);
    //                }
    //                LoadEffect(data.uniqueSkill.listEvents[i].recepterEffect);
    //            }
    //        }
    //    }
    //}
}
