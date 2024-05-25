using System;
using System.Collections.Generic;
using System.Linq;
using TicTocGuardians.Scripts.Assets;
using TicTocGuardians.Scripts.Interface;
using UniRx;
using UniRx.Triggers;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

namespace TicTocGuardians.Scripts.Game
{
    [CustomEditor(typeof(MovableLevelObject))]
    public class MovableLevelObjectEditor : Editor
    {
        private Vector2 _scrollPosition;
        public virtual void OnSceneGUI()
        {
            MovableLevelObject obj = (MovableLevelObject)target;

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
    [ExecuteAlways]
    public class MovableLevelObject : LevelObject, IReactable
    {
        public enum Playback
        {
            OnceForward,
            LoopForward,
            OncePingpong,
            LoopPingpong,
        }
        [Space(10)]
        [Header("Movable Level Object")]
        public DynamicLevelObject instance;
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

            instance.GetComponentInChildren<MeshCollider>().OnCollisionEnterAsObservable().Subscribe(other =>
            {
                Debug.Log(other.contacts[0].normal);
                if (other.collider.GetComponent<IDynamic>() != null && other.contacts[0].normal.y<=-0.7f)
                {
                    //other.collider.transform.parent = instance.GetComponentInChildren<MeshCollider>().transform;
                }
            });

            instance.GetComponentInChildren<MeshCollider>().OnCollisionExitAsObservable().Subscribe(other =>
            {
                if (other.collider.GetComponent<IDynamic>() != null)
                {
                    //other.collider.transform.parent = GameManager.instance.currentPlace.worldOrigin;
                }
            });

            //points = pointsOrigin.GetComponentsInChildren<Transform>();
            instance.transform.position = points[0].position;
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetMove(bool move)
        {
            isMove = move;
        }

        public override LevelObjectAsset Serialize(PlaceAsset parent)
        {
            var asset = new LevelObjectAsset();
            asset.name = gameObject.name;
            asset.position = transform.position;
            asset.eulerAngles = transform.eulerAngles;
            asset.scale = transform.localScale;

            asset.modelPrefab = modelPrefab;

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

            return asset;
        }

        public override void Deserialize(LevelObjectAsset asset)
        {
            gameObject.name = asset.name;
            transform.position = asset.position;
            transform.eulerAngles = asset.eulerAngles;
            transform.localScale = asset.scale;
            modelPrefab = asset.modelPrefab;
            instance.modelPrefab = modelPrefab;
            instance.Initialize(modelPrefab);

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

        public override void Initialize(GameObject modelPrefab)
        {
            this.modelPrefab = modelPrefab;
            instance.Initialize(modelPrefab);
        }

        public virtual void React()
        {
            isMove = !isMove;
        }

        public void OnDrawGizmos()
        {
            var mesh = instance.GetComponentInChildren<MeshFilter>().sharedMesh;
            for (int i = 0; i < points.Length; i++)
            {
                var grean = Color.cyan;
                grean.a = 0.5f;
                Gizmos.color = grean;
                Gizmos.DrawWireMesh(mesh, points[i].position);

                if (i < points.Length - 1)
                {
                    var from = points[i].position;
                    var to = points[i + 1].position;
                    var direction = Vector3.Normalize(to - from);

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(from, to);
                    Handles.color = Color.yellow;
                    var size = mesh.bounds.size.x;
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
            if (playback != Playback.OnceForward)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(from1, to1);
                Handles.color = Color.yellow;
                var size1 = mesh.bounds.size.x;
                Handles.ArrowHandleCap(0, from1, Quaternion.LookRotation(direction1), size1, EventType.Repaint);
                GUIStyle labelStyle1 = new GUIStyle();
                labelStyle1.fontSize = 20;
                labelStyle1.normal.textColor = Color.green;
                Handles.Label(points[points.Length - 1].position, (points.Length - 1).ToString(), labelStyle1);
            }
        }
    }
}
