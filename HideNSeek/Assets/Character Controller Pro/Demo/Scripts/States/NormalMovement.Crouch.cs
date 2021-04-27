using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Implementation;

namespace Lightbug.CharacterControllerPro.Demo
{

    public partial class NormalMovement
    {
        public CrouchParameters crouchParameters = new CrouchParameters();

        protected bool wantToCrouch = false;
        protected bool isCrouched = false;

        public void InitCrouch()
        {
            float minshrinkHeightRatio = CharacterActor.BodySize.x / CharacterActor.BodySize.y;
            crouchParameters.heightRatio = Mathf.Max(minshrinkHeightRatio, crouchParameters.heightRatio);
        }
    }
}