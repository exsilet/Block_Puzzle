using System.Threading.Tasks;
using GamingMonks.Utils;
using UnityEngine;

namespace GamingMonks.Features
{
    public class MusicalPlayer : MonoBehaviour
    {        
        private void Awake()
        {            
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            float size = GamePlayUI.Instance.gamePlaySettings.globalLevelModeBlockSize;
            rectTransform.sizeDelta = new Vector2(size, size);
        }

        public void ProduceMusicalNote()
        {
            Produce();
        }
        
        /// <summary>
        /// This functions will search for a filled block and then it will produce a musical node for that.
        /// </summary>
        public async void Produce()
        {
            if (GamePlayUI.Instance.currentLevel.ConveyorBelts.enabled)
            {
                await Task.Delay(System.TimeSpan.FromSeconds(1f));
            }
            else
            {
                await Task.Delay(System.TimeSpan.FromSeconds(0.5f));
            }
                        
            Block musicalNoteBlock = GamePlay.Instance.GetRandomIsFilledBlock();
            if (musicalNoteBlock == null || GamePlay.Instance.musicNoteCount<=0)
            {
                return;
            }
            else
            {
                musicalNoteBlock.SetBlockSpriteType(SpriteType.MusicalNode);
                GamePlay.Instance.blockers.Add(musicalNoteBlock);

                GamePlay.Instance.musicNoteCount -= 1;
                GameObject musicalNote = Instantiate(GamePlay.Instance.musicalNote,transform.position,
                                                    Quaternion.identity, GamePlay.Instance.boardGenerator.transform); 
                musicalNote.transform.localScale = Vector3.one;
                musicalNote.SetActive(true);
                MoveTowardsTarget(musicalNote.transform, musicalNoteBlock);
            }
            
            if (GamePlay.Instance.musicNoteCount == 0)
            {
                GamePlay.Instance.ClearAllBlockMusicDiscs();
            }
        }
        
        /// <summary>
        /// This function will move the produced musical node towards the target and it will place on that. 
        /// </summary>
        /// <param name="musicNote"> It is the instantiated music node that will move</param>
        /// <param name="b"> it's a target block where produced musical node will get placed</param>
        private async void MoveTowardsTarget(Transform musicNote , Block b)
        {
            float time = 0;
            while (time < 0.35)
            {
                await Task.Delay(System.TimeSpan.FromSeconds(0.02f));

                time += Time.deltaTime;
                musicNote.position = Vector3.Lerp(musicNote.position, b.gameObject.transform.position, time);
            }

            b.blockImage.enabled = true;
            b.blockImage.color = b.blockImage.color.WithNewA(1);
            b.isAvailable = false;
            b.isFilled = true;
            b.blockImageLayer1.enabled = true;
            b.blockImageLayer1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.MusicalNode.ToString());
            b.blockImageLayer1.color = b.blockImage.color.WithNewA(1);
            Destroy(musicNote.gameObject);
        }
    }
}
