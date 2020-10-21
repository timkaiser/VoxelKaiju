using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static GameManager;

namespace Character
{
    public class CharacterCamera : MonoBehaviour
    {
        [SerializeField] 
        private GameObject mainCamera;
        CharacterController cameraController;
        [SerializeField]
        PlayerInputManager inputManager; //warning is not correct
        GameObject cameraReference;


        public bool invertCameraVertical;

        [SerializeField]
        private float cameraRotationSpeed = 1.0f;

        [SerializeField]
        private float cameraFollowSpeed = 1.0f;

        [SerializeField]
        bool animateCharacter = false;

        public GameObject target = null; //pivot, camera is rotating around and looks at
        private Vector3 targetPosition;

        //Camera input
        float inputHorizontalRight = 0.0f;
        float inputVerticalRight = 0.0f;
        float inputHorizontalLeft = 0.0f;

        [SerializeField]
        float delta = 0.05f;

        [SerializeField]
        private float maxAngle = 20.0f;
        [SerializeField]
        private float minAngle = 1.0f;
        private float beginAngle;
        [SerializeField]
        private float defaultDistanceZ = 6.0f;
        [SerializeField]
        private float defaultDistanceY = 3.5f;
        private float defaultDistance;
        private float minimumDistance;

        // Start is called before the first frame update
        void Start()
        {
            mainCamera = Camera.main.gameObject;
            Assert.IsNotNull(mainCamera, "Camera is missing on character.");
            cameraController = mainCamera.GetComponent<CharacterController>();
            Assert.IsNotNull(cameraController, "Camera is missing the character controller.");
            inputManager = PlayerInputManager.Instance;
            Assert.IsNotNull(inputManager, "Input manager not set in CharacterCamera script.");
            Assert.IsNotNull(target, "Camera missing a target.");
            if (cameraReference == null)
            {
                cameraReference = Instantiate(new GameObject("cameraReference"));
            }
            
            targetPosition = target.transform.position;
            mainCamera.transform.LookAt(target.transform);
            beginAngle = mainCamera.transform.rotation.eulerAngles.x;
            cameraReference.transform.position = new Vector3(target.transform.position.x, target.transform.position.y + defaultDistanceY, target.transform.position.z + defaultDistanceZ);
            defaultDistance = Vector3.Distance(target.transform.position, cameraReference.transform.position);
            minimumDistance = defaultDistance * 0.75f;
        }

        // Update is called once per frame
        void Update()
        {
            if (GameManager.Instance.GameState.Equals(GameStates.Paused) || GameManager.Instance.GameState.Equals(GameStates.Scores))
                return;

            //clamp camera in the correct area
            while (cameraReference.transform.rotation.eulerAngles.x > maxAngle)
            {
                cameraReference.transform.RotateAround(target.transform.position, cameraReference.transform.right, -0.01f);
            }
            while (cameraReference.transform.rotation.eulerAngles.x < minAngle)
            {
                cameraReference.transform.RotateAround(target.transform.position, cameraReference.transform.right, 0.01f);
            }

            inputHorizontalRight = AbsThreshold(inputManager.GetHorizontalRight(),0.1f);
            inputVerticalRight = AbsThreshold(inputManager.GetVerticalRight(), 0.1f);
            inputHorizontalLeft = inputManager.GetHorizontalLeft();

            if (!invertCameraVertical)
            {
                inputVerticalRight *= -1;
            }

            float angleUpdateHorizontal = inputHorizontalRight * cameraRotationSpeed * Time.deltaTime * 25f;
            float angleUpdateVertical = inputVerticalRight * cameraRotationSpeed * Time.deltaTime * 25f;


            //todo: subtraction in Anglespace
            float currentAngle = (mainCamera.transform.rotation.eulerAngles.x + angleUpdateVertical);
            if (currentAngle > minAngle && currentAngle < maxAngle)
            {
                cameraReference.transform.RotateAround(target.transform.position, mainCamera.transform.right, angleUpdateVertical);
                //cameraReference.transform.Translate(new Vector3(0.0f, 0.0f, -inputVerticalRight * 0.1f));
            }
                
            cameraReference.transform.RotateAround(target.transform.position, mainCamera.transform.up, angleUpdateHorizontal);
            //fix z rotation
            Quaternion q = cameraReference.transform.rotation;
            q.eulerAngles = new Vector3(q.eulerAngles.x, q.eulerAngles.y, 0);
            cameraReference.transform.rotation = (q);

            //Camera Follow
            Vector3 current = mainCamera.transform.position;
            Vector3 targetPos = mainCamera.transform.position + (target.transform.position - targetPosition);
            cameraReference.transform.Translate(target.transform.position - targetPosition, Space.World);
            targetPosition = target.transform.position;

            //try to come closer, after being blocked
            /*if (Vector3.Distance(current, target.transform.position) > minimumDistance || animateCharacter)
            {
                //follow up fast
                Vector3 smoothDistance = cameraReference.transform.position - current;
                cameraController.Move(smoothDistance);
            }
            else
            {
                cameraReference.transform.position = mainCamera.transform.position;
            }*/

            //follow up fast
            Vector3 smoothDistance = cameraReference.transform.position - current;
            cameraController.Move(smoothDistance);

            cameraReference.transform.LookAt(target.transform);
            mainCamera.transform.LookAt(target.transform);
        }

        public void OnApplicationQuit()
        {
            Destroy(cameraReference);
        }

        public void SetCameraRotationSpeed(float speed) { cameraRotationSpeed = speed; }

        public Vector3 getCameraDirectionForward()
        {
            Vector3 forward = mainCamera.transform.forward;
            forward.y = 0;
            return forward;
        }

        public Vector3 getCameraDirectionRight()
        {
            Vector3 right = mainCamera.transform.right;
            right.y = 0;
            return right;
        }

        private float AbsThreshold(float f, float threshold)
        {
            return (Mathf.Abs(f) > threshold) ? f : 0.0f;
        }

        public void UpdateDefaultDistances(float scale)
        {
            cameraReference.transform.position = new Vector3(target.transform.position.x, target.transform.position.y + defaultDistanceY, target.transform.position.z + defaultDistanceZ);
            defaultDistanceZ = Mathf.Lerp(defaultDistanceZ, defaultDistanceZ* scale, 0.1f * Time.deltaTime);
            defaultDistanceY = Mathf.Lerp(defaultDistanceY, defaultDistanceY* scale, 0.1f * Time.deltaTime);
            minimumDistance = Mathf.Lerp(minimumDistance, minimumDistance * scale, 0.1f * Time.deltaTime);
            minAngle = Mathf.Lerp(minAngle, minAngle + 20f, 0.1f * Time.deltaTime);
            maxAngle = Mathf.Lerp(maxAngle, maxAngle + 20f, 0.1f * Time.deltaTime);
            defaultDistance = Vector3.Distance(target.transform.position, cameraReference.transform.position);
        }

    }
}
