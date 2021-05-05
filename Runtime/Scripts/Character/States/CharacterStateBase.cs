using UnityEngine;
using Cinemachine;

namespace ThirdPersonController
{
    public abstract class CharacterStateBase : ICharacterState
    {
        public static readonly ICharacterState GROUNDED_STATE = new GroundedCharacterState();
        public static readonly ICharacterState JUMPING_STATE = new JumpingCharacterState();
        public static readonly ICharacterState IN_AIR_STATE = new InAirCharacterState();

        public virtual void OnEnter(Character character) { }

        public virtual void OnExit(Character character) { }

        public virtual void Update(Character character)
        {
            character.ApplyGravity();

            if(character.input == null)
            {
                Debug.LogWarning("Character has No CharacterInput set up");
                return;          
            }

            character.MoveVector = GetMovementInput(character.input, character.VirtualCamera.transform, character.VirtualCamera.LookAt.transform);
        }

        public virtual void ToState(Character character, ICharacterState state)
        {
            character.CurrentState.OnExit(character);
            character.CurrentState = state;
            character.CurrentState.OnEnter(character);
        }

        Vector3 GetMovementInput(CharacterInput input, Transform camera, Transform lookAt)
        {
            Vector3 moveVector;
            Vector2 move = input.move;

            float horizontalAxis = move.x;
            float verticalAxis = move.y;

            if (camera != null)
            {
                Vector3 cameraForward = camera.forward;

                if(lookAt != null) // If we have a lookAt, use it for forward computation instead of camera transform forward
                {
                    // Calculate the move vector relative to camera rotation
                    cameraForward = (lookAt.position - camera.position);
                    cameraForward = new Vector3(cameraForward.x, 0, cameraForward.z).normalized; // Get Horizontal Direction (Normalized)
                }

                Vector3 cameraRight = Vector3.Cross(camera.up, cameraForward);
                moveVector = (cameraForward * verticalAxis + cameraRight * horizontalAxis);
            }
            else
            {
                // Use world relative directions
                Debug.LogWarning("PlayerInput.GetMovementInput : No Relative Camera Found.");
                moveVector = (Vector3.forward * verticalAxis + Vector3.right * horizontalAxis);
            }

            if (moveVector.magnitude > 1f)
            {
                moveVector.Normalize();
            }

            return moveVector;
        }
    }
}
