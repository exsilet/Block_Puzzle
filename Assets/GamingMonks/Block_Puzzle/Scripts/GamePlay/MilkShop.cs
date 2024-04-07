using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingMonks
{
    public class MilkShop: MonoBehaviour
    {
        [Tooltip("Delay time for bottle destruction.")]
        [SerializeField] private float destroyTime;
        
        // bottles that milk shop contains.
        public List<GameObject> bottles;
        
        // Grid's blocks that are occupied by the milk shop.
        public List<Block> occupiedBlocks;

        public float originalScale = 0.15f;
        private void Awake()
        {
            BoardSize boardSize = GamePlayUI.Instance.GetBoardSize();

            int rowSize = (int)boardSize;
            int columnSize = (int)boardSize;

            // Fetched the size of block that should be used.
            float blockSize =  GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSize;
            // Fetched the space between blocks that should be used.
            float blockSpace = GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSpace;
            
            Vector3 xVector = new Vector3((rowSize * blockSize) + ((rowSize - 1) * blockSpace),
                (columnSize * blockSize) + ((columnSize - 1) * blockSpace), 1)  * originalScale;
            transform.localScale = xVector / ((rowSize * 81) + ((rowSize - 1) * 0));
        }

        // Blocks that are occupied by the milk shop will be marked as unavailable,
        // so that they can't be used for shape placing. 
        public IEnumerator MarkOccupiedBlocksUnAvail(Block rootBlock)
        {
            yield return new WaitForSeconds(.5f);
            
            List<Block> row = new List<Block>();
            row = GamePlay.Instance.allRows[rootBlock.RowId];
            List<Block> nextRow = new List<Block>();
            nextRow = GamePlay.Instance.allRows[rootBlock.RowId +1];
            
            
            
            occupiedBlocks = new List<Block>
            {
                rootBlock,
                row[rootBlock.ColumnId + 1],
                nextRow[rootBlock.ColumnId],
                nextRow[rootBlock.ColumnId + 1]
            };
            
            if(occupiedBlocks.Count > 0)
            {
                foreach (Block block in occupiedBlocks)
                {
                    block.thisCollider.enabled = false;
                    block.isAvailable = false;
                    block.isFilled = true;
                    block.hasMilkShop = true;
                    block.milkShop = this;
                    GamePlay.Instance.blockers.Add(block);
                    block.SetBlockSpriteType(SpriteType.MilkBottle);
                }
            }
        }
        
        // Blocks that are occupied by the milk shop will be marked as available,
        // so that they can be used for shape placing. 
        private void MarkOccupiedBlocksAvail()
        {
            if(occupiedBlocks.Count > 0)
            {
                foreach (Block block in occupiedBlocks)
                {
                    block.thisCollider.enabled = true;
                    block.isAvailable = true;
                    block.isFilled = false;
                    block.hasMilkShop = false;
                    block.milkShop = null;
                    GamePlay.Instance.blockers.Remove(block);
                    block.SetBlockSpriteType(SpriteType.Empty);
                }
            }
        }
        
        // One milk bottle will be removed from milk shop when line completes.
        public void CollectMilkBottle()
        {
            if (bottles.Count > 0)
            {
                GameObject bottle = bottles[0];
                bottles.RemoveAt(0); 
                
                TargetController.Instance.UpdateTarget(bottle.transform, SpriteType.MilkBottle);
                Destroy(bottle, destroyTime);
            }
            if (bottles.Count <= 0)
            {
                MarkOccupiedBlocksAvail();
                GamePlay.Instance.instantiatedGameObjects.Remove(gameObject);
                Destroy(gameObject, .2f);
            }
        }
    }
}