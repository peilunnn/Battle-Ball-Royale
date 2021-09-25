using UnityEngine;

namespace DM
{
    public class CameraManager : MonoBehaviour
    {
        public float followSpeed = 9f;  //defines camera speed that follows character.
        public float mouseSpeed = 2f;   //defines mouse's control speed.
        public float controllerSpeed = 7f;  //defines joypad's control speed.

        public Transform target;    //stores the target for camera.

        [HideInInspector] public Transform pivot;     //pivot for camera's rotation.
        [HideInInspector] public Transform camTrans;  //stores camera's root transform.

        float turnSmoothing = 0.1f;     //defines how smooth this camera moves.
        public float minAngle = -35f;   //minimum vertical angle.
        public float maxAngle = 35f;    //maximum vertical angle.

        float smoothX;
        float smoothY;
        float smoothXVelocity;
        float smoothYVelocity;
        public float lookAngle;
        public float tiltAngle;

        Transform mainCamera;
        int defaultMaskIndex;
        int crosshairMaskIndex;
        GameObject obstruction;
        RaycastHit[] hits = new RaycastHit[0];
        float camToPlayerDistance;
        float camToWallDistance;
        float offset = 10f;

        void Start()
        {
            defaultMaskIndex = LayerMask.NameToLayer("Default");
            crosshairMaskIndex = LayerMask.NameToLayer("Crosshair");
        }

        public void Init(Transform t)   //Initiallize camera settings.
        {
            target = t;
            camTrans = Camera.main.transform;
            pivot = camTrans.parent;

            mainCamera = GameObject.Find("Main Camera").transform;
        }

        public void FixedTick(float d)   //Getting camera inputs and updates camera's stats.
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");

            float c_h = Input.GetAxis("RightAxis X");
            float c_v = Input.GetAxis("RightAxis Y");

            float targetSpeed = mouseSpeed;


            if (c_h != 0 || c_v != 0)
            {
                h = c_h;
                v = c_v;
                targetSpeed = controllerSpeed;
            }

            FollowTarget(d);
            HandleRotations(d, v, h, targetSpeed);
            OnViewObstructed();
        }

        void FollowTarget(float d)  //defines how camera follows the target.
        {
            float speed = d * followSpeed;
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            transform.position = targetPosition;
        }

        void HandleRotations(float d, float v, float h, float targetSpeed)  //defines the rotation of camera.
        {
            if (turnSmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXVelocity, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYVelocity, turnSmoothing);
            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);

            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

        }

        public static CameraManager singleton;

        private void Awake() => singleton = this;

        void OnViewObstructed()
        {
            // if wall and/or ceiling not blocking anymore, re-enable them
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider && (hit.collider.tag == "Wall" || hit.collider.tag == "Ceiling"))
                {
                    obstruction = hit.transform.gameObject;
                    obstruction.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    obstruction.layer = defaultMaskIndex;
                }
            }

            camToPlayerDistance = Vector3.Distance(mainCamera.position, target.position);

            Vector3 camToPlayerDirection = target.position - mainCamera.position;
            Ray ray = new Ray(mainCamera.position, camToPlayerDirection);

            hits = Physics.RaycastAll(mainCamera.position, camToPlayerDirection, camToPlayerDistance);
            foreach (RaycastHit hit in hits)
            {
                if ((hit.collider.tag != "Wall") && hit.collider.tag != "Ceiling")
                    return;

                camToWallDistance = Vector3.Distance(mainCamera.position, hit.transform.position);

                // keep shadows only to give the illusion that the object is still there 
                if (camToWallDistance >= camToPlayerDistance - offset)
                {
                    obstruction = hit.transform.gameObject;
                    obstruction.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                    obstruction.layer = crosshairMaskIndex;
                }
            }
        }
    }
}


