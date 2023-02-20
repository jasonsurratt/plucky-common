using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace knockback
{
    public class MultiTag : MonoBehaviour
    {
        /// Do NOT change the order or remove items as existing prefabs will be messed up. Just
        /// append to the end and add comments & rename obsolete tags
        public enum Tag
        {
            Enemy,
            EnemyTarget,
            Obstacle,
            PersistentWeapon,
            Player,
            /// <summary>
            /// The object is currently being destroyed.
            /// </summary>
            Destroying,
            /// <summary>
            /// LevelComplete means these objects should be displayed when the level is over,
            /// either won or lost.
            /// </summary>
            LevelComplete,
            /// Repairable means the object is mechanical in nature and requires repairing not
            /// healing.
            Repairable,
            /// Healable means the object is healable in nature and requires healing not
            /// repairing. If this tag isn't set it is assumed to be healable. Mostly useful in
            /// conjuction with Repairable. E.g. it could be both repairable and healable.
            Healable,
            /// NoDamageColliders means that none of the colliders on this game object should
            /// deal damage. Triggers or otherwise. This only applies to this game object and not
            /// its children/parents.
            NoDamageColliders,
            /// AttackController is placed on all objects that have an AttackController.
            AttackController,
            /// SceneCamera is a camera that can be used when the player isn't active.
            SceneCamera,
            /// PuppetMaster is put on all objects with a PuppetMaster component to make them easy
            /// to find.
            PuppetMaster = 16,
            /// MapMagicIntersection is a manually placed intersection that MapMagic will connect
            /// into a road network.
            MapMagicIntersection = 17,
            /// HumanNpc a presumed friendly NPC
            HumanNpc = 18,
            MainCamera,
            /// The location of a village. This is used for procedurally generating terrain.
            Village = 20,
            /// Unity default tag. I use this on player spawn points.
            Respawn = 1000,
        }

        public const string EnemySpawn = "EnemySpawn";
        public const string EnemyTarget = "EnemyTarget";

        public static Dictionary<Tag, List<GameObject>> dict =
            new Dictionary<Tag, List<GameObject>>();
        static GameObject[] _emptyArray = new GameObject[0];

        public Tag[] tags = null;

        public void Add(Tag t)
        {
            if (HasTag(t)) return;

            if (tags == null)
            {
                tags = new Tag[] { t };
            }
            else
            {
                var newTags = new Tag[tags.Length + 1];
                Array.Copy(tags, newTags, tags.Length);
                newTags[newTags.Length - 1] = t;
            }
            AddTagToDict(t);
        }

        public static void AddTag(GameObject go, Tag t)
        {
            if (go.tag == null || go.CompareTag("Untagged"))
            {
                go.tag = t.ToString();
            }
            else
            {
                var mt = go.GetComponent<MultiTag>();
                if (!mt) mt = go.AddComponent<MultiTag>();
                mt.Add(t);
            }
        }

        void AddTagToDict(Tag t)
        {
            List<GameObject> l;
            if (!dict.TryGetValue(t, out l))
            {
                l = new List<GameObject>();
                dict[t] = l;
            }
            l.Add(gameObject);
        }

        void Awake()
        {
            if (tags != null)
            {
                foreach (Tag t in tags)
                {
                    AddTagToDict(t);
                }
            }
        }

        public void Clear()
        {
            if (tags == null) return;

            foreach (Tag t in tags)
            {
                if (dict.TryGetValue(t, out List<GameObject> l))
                {
                    l.Remove(gameObject);
                    if (l.Count == 0) dict.Remove(t);
                }
            }

            tags = null;
        }

        public bool HasTag(Tag t)
        {
            if (gameObject.CompareTag(t.ToString())) return true;
            if (tags == null) return false;
            // We could use a sorted array, hashset, etc. But typically this will only contain one
            // or two values.
            if (Array.Exists(tags, x => x == t)) return true;

            return false;
        }

        /// <summary>
        /// HasTag returns true if obj either has a "normal" tag, or a multi tag that equals t.
        /// 
        /// This is strongly preferred over using the Unity CompareTag function.
        /// </summary>
        /// <param name="obj">Look for a tag in this object</param>
        /// <param name="t">The tag to look for</param>
        /// <returns>True if the object contains the unity or multitag tag.</returns>
        public static bool HasTag(GameObject obj, Tag t)
        {
            if (obj.CompareTag(t.ToString())) return true;

            var multiTags = obj.GetComponent<MultiTag>();
            if (multiTags && multiTags.HasTag(t)) return true;

            return false;
        }

        void OnDestroy()
        {
            Clear();
        }

        /// <summary>
        /// GetAll returns all active objects with the tag directly on the object and all MultiTag
        /// objects wether they are active or not.
        /// </summary>
        public static IEnumerable<GameObject> GetAll(Tag tag)
        {
            IEnumerable<GameObject> result = null;
            var array = GameObject.FindGameObjectsWithTag(tag.ToString());
            if (array.Length > 0) result = array;

            List<GameObject> values;
            if (dict.TryGetValue(tag, out values))
            {
                // remove any stale entries.
                values.RemoveAll(x => !x);

                if (values.Count == 0)
                {
                    dict.Remove(tag);
                }
                else
                {
                    if (result == null)
                    {
                        result = values;
                    }
                    else
                    {
                        result = result.Concat(values);
                    }
                }
            }
            if (result == null) result = _emptyArray;

            return result;
        }

        public static IEnumerable<GameObject> GetAll(IEnumerable<Tag> tags)
        {
            IEnumerable<GameObject> result = null;
            foreach (Tag t in tags)
            {
                var l = GetAll(t);
                if (l.Count() > 0)
                {
                    if (result == null) result = l;
                    else result = result.Concat(l);
                }
            }

            if (result == null) result = _emptyArray;

            return result;
        }

        public static IEnumerable<GameObject> GetAll(string tag) => GetAll(ToEnum(tag));

        /// <summary>
        /// Get returns all active GameObjects with the specified tag in either MultiTag or
        /// directly on the object.
        /// </summary>
        public static IEnumerable<GameObject> Get(Tag tag)
        {
            return GetAll(tag).Where(x => x.activeInHierarchy);
        }

        /// <summary>
        /// ReplaceTags removes all existing tags and sets the tag to the specified tag.
        /// </summary>
        /// <param name="obj">object to set the tag on</param>
        /// <param name="t">the new tag</param>
        public static void ReplaceTags(GameObject obj, Tag t)
        {
            var multiTags = obj.GetComponent<MultiTag>();
            if (multiTags) multiTags.Clear();
            obj.tag = t.ToString();
        }

        public static void SetUnityTag(MonoBehaviour mb, Tag t) => SetUnityTag(mb.gameObject, t);

        /// <summary>
        /// SetUnityTag sets the primary (Unity) tag to t and if needed, moves the existing tag
        /// into MultiTag.
        /// </summary>
        public static void SetUnityTag(GameObject go, Tag t)
        {
            if (go.tag == null || go.CompareTag("Untagged"))
            {
                go.tag = t.ToString();
            }
            else
            {
                Tag tmp = ToEnum(go.tag);
                go.tag = t.ToString();
                AddTag(go, tmp);
            }
        }

        public static Tag ToEnum(string tagName)
        {
            return (Tag)Enum.Parse(typeof(Tag), tagName);
        }
    }
}
