using System;
using System.Collections.Generic;
using System.Linq;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Assets.LevelAsset;
using TicTocGuardians.Scripts.Game.ETC;
using TicTocGuardians.Scripts.Game.Manager;
using TicTocGuardians.Scripts.Interface;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

namespace TicTocGuardians.Scripts.Game.LevelObjects
{
    [ExecuteAlways]
    public class MovableTileModelLevelObject : ModelLevelObject, IReactable
    {
        public enum Playback
        {
            OnceForward,
            LoopForward,
            OncePingpong,
            LoopPingpong,
            Returning
        }

        [Space(10)]
        [Header("Movable Level Object")]
        public TileModelLevelObject instance;
        public Transform pointsOrigin;


        public Transform[] points;

        [Range(0.1f, 100)]
        public float moveSpeed;
        public Playback playback;
        public bool isMove;
        public float tolerance;

        [SerializeField]
        private int nextPointIndex = 1;


        private bool _reverse = false;
        private Vector3 _velocity;

        public void Update()
        {
#if UNITY_EDITOR
            var children = from child in pointsOrigin.GetComponentsInChildren<Transform>()
                           where child != pointsOrigin.transform
                           select child;
            points = children.ToArray();
#endif
        }

        public void Start()
        {

            this.UpdateAsObservable().Where(_ => isMove)
                .Subscribe(_ => Move()).AddTo(gameObject);

            this.UpdateAsObservable().Where(_ => isMove)
                .Subscribe(_ => instance.transform.position += _velocity * Time.deltaTime).AddTo(gameObject);

            this.UpdateAsObservable().Where(_ => playback==Playback.Returning && !isMove)
                .Subscribe(_ => Return()).AddTo(gameObject);



            var children = instance.GetComponentsInChildren<MeshCollider>();
            foreach (var child in children)
            {
                child.OnCollisionEnterAsObservable().Subscribe(other =>
                {
                    Debug.Log(other.contacts[0].normal);
                    if (other.collider.GetComponent<IMovable>() != null && other.contacts[0].normal.y <= -0.7f)
                    {
                        other.collider.transform.parent = instance.transform;
                    }
                });
                child.OnCollisionExitAsObservable().Subscribe(other =>
                {
                    if (other.collider.GetComponent<IMovable>() != null)
                    {
                        other.collider.transform.parent = LevelOrigin.Instance.transform;
                    }
                });
            }

            
            instance.transform.position = points[0].position;
        }



        private void Return()
        {
            if (nextPointIndex > 0)
            {
                var targetPoint = points[nextPointIndex-1];
                var diffrence = targetPoint.position - instance.transform.position;
                var direction = diffrence.normalized;
                var distance = diffrence.magnitude;
                if (distance > tolerance)
                {
                    _velocity = direction * moveSpeed;
                }
                else
                {
                    nextPointIndex--;
                }

                instance.transform.position += _velocity * Time.deltaTime;
            }
        }

        private void Move()
        {
            var targetPoint = points[nextPointIndex];
            var diffrence = targetPoint.position - instance.transform.position;
            var direction = diffrence.normalized;
            var distance = diffrence.magnitude;
            if (distance > tolerance)
            {
                _velocity = direction * moveSpeed;
            }
            else
            {
                NextPoint();
            }
        }

        public void NextPoint()
        {
            switch (playback)
            {
                case Playback.OnceForward:
                    if (nextPointIndex < points.Length - 1)
                    {
                        nextPointIndex++;
                    }
                    else
                    {
                        SetMove(false);
                    }
                    break;
                case Playback.LoopForward:
                    if (nextPointIndex < points.Length - 1)
                    {
                        nextPointIndex++;
                    }
                    else
                    {
                        nextPointIndex = 0;
                    }
                    break;
                case Playback.OncePingpong:
                    if (nextPointIndex == points.Length - 1 && !_reverse)
                    {
                        nextPointIndex--;
                        _reverse = true;
                    }
                    else if (nextPointIndex == 0 && _reverse)
                    {
                        SetMove(false);
                    }
                    else if (!_reverse)
                    {
                        nextPointIndex++;
                    }
                    else if (_reverse)
                    {
                        nextPointIndex--;
                    }

                    break;
                case Playback.LoopPingpong:
                    if (nextPointIndex == points.Length - 1 && !_reverse)
                    {
                        nextPointIndex--;
                        _reverse = true;
                    }
                    else if (nextPointIndex == 0 && _reverse)
                    {
                        nextPointIndex++;
                        _reverse = false;
                    }
                    else if (!_reverse)
                    {
                        nextPointIndex++;
                    }
                    else if (_reverse)
                    {
                        nextPointIndex--;
                    }
                    break;
                case Playback.Returning:
                    if (nextPointIndex < points.Length - 1)
                    {
                        nextPointIndex++;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetMove(bool move)
        {
            isMove = move;
        }

        public override LevelObjectAsset Serialize(LevelAsset parent)
        {
            var asset = base.Serialize(parent);

            List<Vector3DataAsset> pointDataList = new List<Vector3DataAsset>();
            var points = from child in pointsOrigin.GetComponentsInChildren<Transform>()
                         where child != pointsOrigin.transform
                         select child;

            foreach (var point in points)
            {
                var pointData = Vector3DataAsset.Create(point.gameObject.name, point.position) as Vector3DataAsset;
                pointDataList.Add(pointData);
                AssetDatabase.AddObjectToAsset(pointData, parent);
            }

            asset.AddData(parent, PointArrayDataAsset.Create("points", pointDataList.ToArray()));
            asset.AddData(parent, FloatDataAsset.Create("moveSpeed", moveSpeed));
            asset.AddData(parent, IntegerDataAsset.Create("playback", (int)playback));
            asset.AddData(parent, BoolDataAsset.Create("isMove", isMove));
            asset.AddData(parent, FloatDataAsset.Create("tolerance", tolerance));
            asset.AddData(parent, LevelObjectDataAsset.Create("instance", instance.Serialize(parent)));


            return asset;
        }

        public override void Deserialize(LevelObjectAsset asset)
        {
            base.Deserialize(asset);
            modelPrefab= (GameObject)asset.GetValue("modelPrefab");
            instance.Deserialize((LevelObjectAsset)asset.GetValue("instance"));
            
            var allChildren = pointsOrigin.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (child != null && child != pointsOrigin)
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            Vector3DataAsset[] pointDataArray = (Vector3DataAsset[])asset.GetValue("points");

            foreach (var pointData in pointDataArray)
            {
                var a = Instantiate(GlobalLevelSetting.instance.dummy, pointsOrigin);
                a.gameObject.name = pointData.name;
                a.transform.position = pointData.value;
            }
            var childrenPoints = from child in pointsOrigin.GetComponentsInChildren<Transform>()
                                 where child != pointsOrigin.transform
                                 select child;
            points = childrenPoints.ToArray();

            moveSpeed = (float)asset.GetValue("moveSpeed");
            playback = (Playback)((int)asset.GetValue("playback"));
            isMove = (bool)asset.GetValue("isMove");
            tolerance = (float)asset.GetValue("tolerance");
        }

        public override void Initialize(GameObject modelObject)
        {
            modelPrefab = modelObject;
            instance.Initialize(modelObject);
        }

        public virtual void React()
        {
            isMove = !isMove;
        }

        public void OnDrawGizmos()
        {
            var objects = instance.GetComponentsInChildren<StaticModelLevelObject>();
            float size = 1.0f;
            foreach (var x in objects)
            {
                var mesh = instance.GetComponentInChildren<MeshFilter>().sharedMesh;
                for (int i = 0; i < points.Length; i++)
                {
                    var grean = Color.cyan;
                    grean.a = 0.5f;
                    Gizmos.color = grean;

                    Matrix4x4 matrix = transform.localToWorldMatrix * Matrix4x4.TRS(x.transform.localPosition,x.transform.localRotation,x.transform.localScale) * Matrix4x4.Translate(points[i].localPosition);
                    Gizmos.DrawWireMesh(mesh, matrix.GetPosition(), matrix.rotation);
                    size = mesh.bounds.size.x;
                }
            }

            for (int i = 0; i < points.Length; i++)
            {
                if (i < points.Length - 1)
                {
                    var from = points[i].position;
                    var to = points[i + 1].position;
                    var direction = Vector3.Normalize(to - from);

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(from, to);
                    Handles.color = Color.yellow;

                    Handles.ArrowHandleCap(0, from, Quaternion.LookRotation(direction), size, EventType.Repaint);
                }
                GUIStyle labelStyle = new GUIStyle();
                labelStyle.fontSize = 20;
                labelStyle.normal.textColor = Color.green;
                Handles.Label(points[i].position, i.ToString(), labelStyle);
            }


            Vector3 from1 = Vector3.zero;
            Vector3 to1 = Vector3.zero;
            Vector3 direction1 = Vector3.zero;
            switch (playback)
            {
                case Playback.LoopForward:
                    from1 = points[points.Length - 1].position;
                    to1 = points[0].position;
                    direction1 = Vector3.Normalize(to1 - from1);

                    break;
                case Playback.OncePingpong:
                    from1 = points[points.Length - 1].position;
                    to1 = points[points.Length - 2].position;
                    direction1 = Vector3.Normalize(to1 - from1);
                    break;
                case Playback.LoopPingpong:
                    from1 = points[points.Length - 1].position;
                    to1 = points[points.Length - 2].position;
                    direction1 = Vector3.Normalize(to1 - from1);
                    break;
            }
            if (playback != Playback.OnceForward && playback != Playback.Returning)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(from1, to1);
                Handles.color = Color.yellow;
                Handles.ArrowHandleCap(0, from1, Quaternion.LookRotation(direction1), size, EventType.Repaint);
                GUIStyle labelStyle1 = new GUIStyle();
                labelStyle1.fontSize = 20;
                labelStyle1.normal.textColor = Color.green;
                Handles.Label(points[points.Length - 1].position, (points.Length - 1).ToString(), labelStyle1);
            }
        }
    }
    [CustomEditor(typeof(MovableTileModelLevelObject))]
    public class MovableTileLevelObjectEditor : Editor
    {
        private Vector2 _scrollPosition;
        public virtual void OnSceneGUI()
        {
            MovableTileModelLevelObject obj = (MovableTileModelLevelObject)target;

            var mesh = obj.instance.GetComponentInChildren<MeshFilter>().sharedMesh;
            SceneView sceneView = SceneView.lastActiveSceneView;
            var rot = Quaternion.LookRotation(sceneView.camera.transform.forward, sceneView.camera.transform.up);
            for (int i = 0; i < obj.points.Length; i++)
            {
                float size = mesh.bounds.size.x;
                Vector3 position = obj.points[i].position + mesh.bounds.center;
                Handles.color = Color.white;

                if (Handles.Button(position, rot, size, size, Handles.RectangleHandleCap))
                {
                    Selection.activeObject = obj.points[i].gameObject;
                }
            }

        }
    }

}
