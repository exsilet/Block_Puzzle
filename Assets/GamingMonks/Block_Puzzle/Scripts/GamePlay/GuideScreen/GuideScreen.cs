using GamingMonks;
using GamingMonks.Tutorial;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
    public class GuideScreen : MonoBehaviour
    {
        [SerializeField] private Image guideSprite;
        [SerializeField] private TextMeshProUGUI guideMessage;
        LevelSO levelSO;
        private List<GameObject> blockObjectList = new List<GameObject>();
        private List<GameObject> instantiatedObjectList = new List<GameObject>();

        private void Awake()
        {
            if (levelSO == null)
            {
                levelSO = (LevelSO)Resources.Load("LevelSettings");
            }
        }

        private void OnEnable()
        {
            blockObjectList.Clear();
            instantiatedObjectList.Clear();
            SetUpGuide();
            //Invoke("SetUpGuide",2f);
        }

        private void OnDisable()
        {
            //grid feature clear.
            foreach (GameObject block in blockObjectList)
            {
                block.GetComponent<Canvas>().sortingOrder = 1;
                if (block.GetComponent<Block>().spriteType == SpriteType.Bubble)
                {
                    block.GetComponent<Block>().GetBubbleGameObject().GetComponent<Canvas>().sortingOrder = 2;
                }
                else if (block.GetComponent<Block>().spriteType == SpriteType.Kite)
                {
                    block.GetComponentInChildren<Canvas>().sortingOrder = 1;
                }
                else if(block.GetComponent<Block>().conveyorMoverBlock != null)
                {
                    block.GetComponent<Block>().conveyorImage.GetComponent<Canvas>().sortingOrder = 1;
                }
            }
            //instantiated feature clear.
            foreach (GameObject instantiatedObject in instantiatedObjectList)
            {
                instantiatedObject.GetComponent<Canvas>().sortingOrder = 1;
            }
        }


        private void SetUpGuide()
        {
            SpriteType levelGuideSprite = levelSO.Levels[GamePlayUI.Instance.levelToLoad - 1].guide.guideSprite;
            guideSprite.sprite = ThemeManager.Instance.GetBlockSpriteWithTag((levelGuideSprite==SpriteType.IceMachine)?  levelGuideSprite.ToString()+"Stage1":levelGuideSprite.ToString());
            guideMessage.text = levelSO.Levels[GamePlayUI.Instance.levelToLoad - 1].guide.guideMessage;
            if (levelGuideSprite == SpriteType.MilkBottle)//instantiated objects
            {
                foreach (GameObject instantiatedObjects in GamePlay.Instance.instantiatedGameObjects)
                {
                    if (instantiatedObjects.GetComponent<MilkShop>() != null)
                    {
                        instantiatedObjects.GetComponent<Canvas>().sortingOrder = 5;
                        instantiatedObjectList.Add(instantiatedObjects.gameObject);
                    }
                }

            }
            else if (levelGuideSprite == SpriteType.Diamond)
            {
                foreach (GameObject instantiatedObjects in GamePlay.Instance.instantiatedGameObjects)
                {
                    if (instantiatedObjects.GetComponent<Diamond>() != null)
                    {
                        instantiatedObjects.GetComponent<Canvas>().sortingOrder = 5;
                        instantiatedObjectList.Add(instantiatedObjects.gameObject);
                    }
                }
            }
            //else if (levelGuideSprite == SpriteType.ConveyorRight)
            //{
            //    foreach (Block conveyorBlocks in ConveyorBeltController.Instance.conveyorBlocks)
            //    {
            //        if (conveyorBlocks != null)
            //        {
            //            conveyorBlocks.GetComponent<Canvas>().sortingOrder = 5;
            //            instantiatedObjectList.Add(conveyorBlocks.gameObject);
            //        }
            //    }
            //}
            else
            {
                foreach (List<Block> blockList in GamePlay.Instance.allRows)
                {
                    foreach (Block block in blockList)
                    {
                        if (block.spriteType == levelGuideSprite)
                        {
                            block.GetComponent<Canvas>().sortingOrder = 5;
                            if (block.spriteType == SpriteType.Bubble)
                            {
                                block.GetComponent<Block>().GetBubbleGameObject().GetComponent<Canvas>().sortingOrder = 6;
                            }
                            blockObjectList.Add(block.gameObject);
                        }
                        else if(block.spriteType == SpriteType.RedGiftBox || block.spriteType == SpriteType.BlueGiftBox)
                        {
                            block.GetComponent<Canvas>().sortingOrder = 5;
                            blockObjectList.Add(block.gameObject);
                        }
                        else if (block.spriteType == SpriteType.Kite)
                        {
                            block.GetComponentInChildren<Canvas>().sortingOrder = 5;
                        }
                        else if(block.conveyorMoverBlock != null)
                        {
                            block.conveyorImage.GetComponent<Canvas>().sortingOrder = 5;
                            blockObjectList.Add(block.gameObject);
                        }
                    }
                }
            }
        }

        public void OnCancel()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                this.gameObject.Deactivate();
            }
        }
    }
}

