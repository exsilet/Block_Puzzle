﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
    /// <summary>
    /// This script typically checks whether the given block shape can be place on board or not. This script generates a clone of block space 
    /// and tries to place at all the block positions of the block grid to determine it can be placed or. Upon returning the result clone of block 
    /// shape will be destroyed.
    /// </summary>
	public class BlockShapePlacementChecker : MonoBehaviour
    {
        List<Transform> activeBlocks = new List<Transform>();
        Transform shapeClone = null;

        /// <summary>
        /// Checks whether the given block shape can be place on board.
        /// </summary>
        public bool CheckShapeCanbePlaced(BlockShape blockShape)
        {
            activeBlocks = new List<Transform>();
            shapeClone = ((GameObject) Instantiate(blockShape.gameObject, Vector3.zero, Quaternion.identity, transform) as GameObject).transform;
            shapeClone.localScale = Vector3.one - new Vector3(0.1f, 0.1f, 0);
            shapeClone.localEulerAngles = blockShape.transform.localEulerAngles;

            foreach (Transform t in shapeClone) {
                if (t.gameObject.activeSelf)  {
                    activeBlocks.Add(t);
                    t.GetComponent<Image>().enabled = false;
                }
            }

            // Tries to place shape will rotations if rotation is allowed. Will Iterate through all the blocks inside the grid.
            if(GamePlayUI.Instance.currentModeSettings.allowRotation) {
                
                for(int i = 0; i < 4; i++) {
                    shapeClone.transform.localEulerAngles = new Vector3(0,0, (90 * i));
                    foreach (List<Block> blockRow in GamePlay.Instance.allRows) {
                        foreach (Block b in blockRow) {
                            if(b.isAvailable)
                            {
                                bool result = CheckCanPlaceShapeAtPosition(b.transform.position);
                                if (result) {
                                    Destroy(shapeClone.gameObject);
                                    return true;
                                }
                            }
                        }
                    }
                }
            } else {
                foreach (List<Block> blockRow in GamePlay.Instance.allRows) {
                    foreach(Block b in blockRow) {
                        if(b.isAvailable)
                        {
                            bool result = CheckCanPlaceShapeAtPosition(b.transform.position);
                            if(result) {
                                Destroy(shapeClone.gameObject);
                                return true;
                            }
                        }
                    }
                }
            }
            if(shapeClone != null) {
                Destroy(shapeClone.gameObject);
            }
            return false;
        }


        public bool CheckShapeCanbePlacedWithoutRotate(BlockShape blockShape)
        {
            activeBlocks = new List<Transform>();
            shapeClone = ((GameObject)Instantiate(blockShape.gameObject, Vector3.zero, Quaternion.identity, transform) as GameObject).transform;
            shapeClone.localScale = Vector3.one - new Vector3(0.1f, 0.1f, 0);
            shapeClone.localEulerAngles = blockShape.transform.localEulerAngles;

            foreach (Transform t in shapeClone)
            {
                if (t.gameObject.activeSelf)
                {
                    activeBlocks.Add(t);
                    t.GetComponent<Image>().enabled = false;
                }
            }

            // Tries to place shape will rotations if rotation is allowed. Will Iterate through all the blocks inside the grid.
            foreach (List<Block> blockRow in GamePlay.Instance.allRows)
            {
                foreach (Block b in blockRow)
                {
                    if (b.isAvailable)
                    {
                        bool result = CheckCanPlaceShapeAtPosition(b.transform.position);
                        if (result)
                        {
                            Destroy(shapeClone.gameObject);
                            return true;
                        }
                    }
                }
            }
            if (shapeClone != null)
            {
                Destroy(shapeClone.gameObject);
            }
            return false;
        }

        /// <summary>
        /// Checks whether shape can be placed at the current position. 
        /// </summary>
        bool CheckCanPlaceShapeAtPosition(Vector3 blockPosition)
        {
            Transform firstPoint = activeBlocks[0];
            Vector3 positionOffset = (shapeClone.position - firstPoint.position);
            shapeClone.position = (blockPosition + positionOffset);

            List<Block> hittingBlocks = new List<Block>();

            foreach (Transform t in activeBlocks) {
                Block hittingBlock = GetHittingBlock(t);
                if (hittingBlock == null || hittingBlocks.Contains(hittingBlock)) {
                    return false;
                }
                hittingBlocks.Add(hittingBlock);

                if (hittingBlocks.Count == activeBlocks.Count) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///  Returns block that is interecting with current block shape. Returns null if not any.
        /// </summary>
        Block GetHittingBlock(Transform draggingBlock) {
            BlockShape blockShape = draggingBlock.gameObject.GetComponentInParent<BlockShape>();
            if (blockShape.isBlockerStickEnabled && !blockShape.canPlaceOnBlockerStick)
            {
                // return null if hit with BlockerStick
                RaycastHit2D[] hits = Physics2D.BoxCastAll(draggingBlock.position, new Vector2(0.35f, 0.35f), 0, Vector2.zero);
                foreach (RaycastHit2D h in hits)
                {
                    if (h.collider.gameObject.GetComponent<BlockerStick>() != null)
                    {
                        return null;
                    }
                }
            }
            RaycastHit2D hit = Physics2D.Raycast(draggingBlock.position, Vector2.zero, 1);
            if (hit.collider != null && hit.collider.GetComponent<Block>() != null) {
                return hit.collider.GetComponent<Block>();
            }
            return null;
        }
    }
}

