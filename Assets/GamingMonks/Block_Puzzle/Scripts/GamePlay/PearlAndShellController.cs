using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingMonks
{
    public class PearlAndShellController : Singleton<PearlAndShellController>
    {
        public List<Block> shells;
        private bool isConveyorEnabled = false;
        
        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable()
        {
            shells = new List<Block>();
            StoreAllShells();
            isConveyorEnabled = GamePlayUI.Instance.currentLevel.ConveyorBelts.enabled;
            /// Registers game status callbacks.
            GamePlayUI.OnShapePlacedEvent += GamePlayUI_OnShapePlacedEvent;
        }

        private void StoreAllShells()
        {
            shells.Clear();
            foreach (List<Block> blocks in GamePlay.Instance.allRows)
            {
                foreach (Block block in blocks)
                {
                    if (block.spriteType is SpriteType.OpenShell or SpriteType.CloseShell)
                    {
                        shells.Add(block);
                    }
                }
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            /// Uregisters game status callbacks.
            GamePlayUI.OnShapePlacedEvent -= GamePlayUI_OnShapePlacedEvent;
        }
        
        private void GamePlayUI_OnShapePlacedEvent()
        {
            if(isConveyorEnabled)
            {
                StoreAllShells();
            }
            StartCoroutine(ChangeSprites());
        }


        IEnumerator ChangeSprites()
        {
            if (shells.Count <= 0)
            {
                yield break;
            }
           
            yield return new WaitForSeconds(.3f);
            foreach (Block block in shells)
            {
                if (block.justBorn)
                {
                    block.justBorn = false;
                    continue;
                }
                if (block.spriteType == SpriteType.OpenShell)
                {
                    block.SetBlockSpriteType(SpriteType.CloseShell);
                    block.PlaceBlock(SpriteType.CloseShell.ToString());
                    GamePlay.Instance.blockers.Remove(block);
                }
                else if (block.spriteType == SpriteType.CloseShell)
                {
                    block.SetBlockSpriteType(SpriteType.OpenShell);
                    block.PlaceBlock(SpriteType.OpenShell.ToString());
                    GamePlay.Instance.blockers.Add(block);
                }
            }
        }
    }
}