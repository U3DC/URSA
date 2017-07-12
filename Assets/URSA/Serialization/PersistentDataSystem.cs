﻿namespace URSA.Serialization {
    using UnityEngine;
    using System;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using URSA;
    using System.IO;
    using URSA.Serialization.Blueprints;
    using URSA.Internal;
    using URSA.Utility;

    public class PersistentDataSystem : MonoBehaviour {


        #region SINGLETON
        private static PersistentDataSystem _instance;
        public static PersistentDataSystem instance
        {
            get {
                if (!_instance)
                    _instance = GameObject.FindObjectOfType<PersistentDataSystem>();
                return _instance;
            }
        }
        #endregion

        public string FileName = "persistentGameData";
        public string folderPath = "PersistenData";
        public string extension = ".data";

        public static event Action OnPersistentDataLoaded = delegate { };

        public static void MakePersistent(Entity entity) {
            UnityEngine.Object.DontDestroyOnLoad(entity.gameObject);
            entity.transform.parent = instance.transform;
        }


#if UNITY_EDITOR
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_PERSISTENT + URSAConstants.PATH_MENUITEM_PERSISTENT_SAVE)]
#endif
        public static void Save() {
            instance.SaveTo();
        }



#if UNITY_EDITOR
        [MenuItem(URSAConstants.PATH_MENUITEM_ROOT + URSAConstants.PATH_MENUITEM_PERSISTENT + URSAConstants.PATH_MENUITEM_PERSISTENT_LOAD)]
#endif
        public static void Load() {
            instance.LoadFrom();
        }

        public void SaveTo() {
            SaveObject file = SaveSystem.CreateSaveObjectFromPersistenData();
            PersistentDataInfo info = new PersistentDataInfo();
            info.profileName = "profile";
            info.creationDate = DateTime.Now;
            info.data = file;

            string path = PathUtilities.CustomDataPath + "/" + folderPath;

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            SerializationHelper.Serialize(info, path + "/" + FileName + extension, true);
        }

        public void LoadFrom() {
            ClearAnd(completeLoad);
        }

        public void LoadWithoutClear(TextAsset blueprint) {
            var bpLoader = GetComponent<BlueprintLoader>();
            if (!bpLoader) {
                Debug.LogError("Blueprint loader was not found, add it to the PersistentDataSystem");
                return;
            }
            bpLoader.Blueprint = blueprint;
            bpLoader.Load();
        }

        public void ClearAnd(Action and) {
            transform.DestroyChildren();
            this.OneFrameDelay(and);
        }

        public void ClearAndLoadBlueprint(TextAsset blueprint, Action onLoaded) {
            var bpLoader = GetComponent<BlueprintLoader>();
            if (!bpLoader) {
                Debug.LogError("Blueprint loader was not found, add it to the PersistentDataSystem");
                return;
            }
            ClearAnd(() => {
                bpLoader.Blueprint = blueprint;
                bpLoader.Load();
                if (onLoaded != null)
                    onLoaded();
            });
        }

        void completeLoad() {
            string path = PathUtilities.CustomDataPath + "/" + folderPath + "/" + FileName + extension;

            if (File.Exists(path)) {
                var info = SaveSystem.DeserializeAs<PersistentDataInfo>(path);
                SaveSystem.UnboxSaveObject(info.data, transform);
                OnPersistentDataLoaded();
            }
            else
                Debug.LogError("PersistentDataSystem: File at path:" + path + " was not found");
        }

        [Serializable]
        public class PersistentDataInfo {
            public SaveObject data;
            //Add your custom info here
            public string profileName;
            public DateTime creationDate;
        }
    }

}