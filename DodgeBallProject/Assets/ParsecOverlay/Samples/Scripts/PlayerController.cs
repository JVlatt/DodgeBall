using Rewired;
using UnityEngine;

namespace ParsecOverlaySample
{
    public class PlayerController : MonoBehaviour
    {
        public string rewiredPlayerName = "Player0";
        private Player _rewiredPlayer = null;

        public float speed = 5f;

        private void Awake()
        {
            _rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerName);
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 dir = _rewiredPlayer.GetAxis2D("MoveLeftRight", "MoveUpDown");

            Vector3 newPosition = transform.position;
            newPosition.x += dir.x * speed * Time.deltaTime;
            newPosition.y += dir.y * speed * Time.deltaTime;
            transform.position = newPosition;
        }
    }
}


