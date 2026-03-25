using UnityEngine;

namespace DiggingGame.Grid
{
    public class ChestOpen : MonoBehaviour
    {
        [SerializeField] private Camera m_camera, c_camera;

        private void Start()
        {
            //m_camera.rect = new Rect(Vector2.zero, new Vector2(0.5f, 1f));
            //c_camera.rect = new Rect(new Vector2(0.5f, 0f), Vector2.one);
        }
    }
}