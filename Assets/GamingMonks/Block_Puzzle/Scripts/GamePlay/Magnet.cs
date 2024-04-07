using System.Collections.Generic;
using GamingMonks;
using DigitalRuby.LightningBolt;
using System.Collections;
using UnityEngine;

public class Magnet : Singleton<Magnet>
{
    public LightningBoltScript lightningBoltPrefab;
    public bool isRowCompletedByPlacingMagnet { get; private set; }
    private bool isBlockerStickEnabled = false;

    public void CheckForRowOrColoumClear(int rowId, int coloumId)
    {
        isBlockerStickEnabled = GamePlayUI.Instance.currentLevel.BlockerStick.enabled;

        if(CanRowClear(rowId) && CanColoumClear(coloumId))
        {
            isRowCompletedByPlacingMagnet = true;
            //GamePlay.Instance.ClearRowByMagnet(rowId);
            GamePlay.Instance.ClearRow(rowId);
            PlayRowBreakingEffectByMagnet(rowId);

            //GamePlay.Instance.ClearColoumByMagnet(coloumId);
            GamePlay.Instance.ClearColoum(coloumId);
            PlayColoumBreakingEffectByMagnet(coloumId);

            if(isBlockerStickEnabled)
            {
                StartCoroutine(BlockerStickSpwaner.Instance.DestroyVerticalBlockerStick(rowId));
                StartCoroutine(BlockerStickSpwaner.Instance.DestroyHorizontalBlockerStick(coloumId));
            }
        }
        else if(CanRowClear(rowId))
        {
            isRowCompletedByPlacingMagnet = true;
            GamePlay.Instance.ClearRow(rowId);
            PlayRowBreakingEffectByMagnet(rowId);

            if (isBlockerStickEnabled)
            {
                StartCoroutine(BlockerStickSpwaner.Instance.DestroyVerticalBlockerStick(rowId));
            }
            
        }
        else if(CanColoumClear(coloumId))
        {
            isRowCompletedByPlacingMagnet = true;
            //GamePlay.Instance.ClearColoumByMagnet(coloumId);
            GamePlay.Instance.ClearColoum(coloumId);
            PlayColoumBreakingEffectByMagnet(coloumId);

            if (isBlockerStickEnabled)
            {
                StartCoroutine(BlockerStickSpwaner.Instance.DestroyHorizontalBlockerStick(coloumId));
            }
        }
    }

    public IEnumerator CheckRowColumnCompleteForMagnetWithIce(int rowId,int columnID)
    {
        yield return new WaitForSeconds(0.0000001f);
        CheckForRowOrColoumClear(rowId, columnID);
    }

    public void SetIsRowCompletedByPlacingMagnet(bool status)
    {
        isRowCompletedByPlacingMagnet = status;
    }

    private bool CanRowClear(int rowID)
    {

        if (!GamePlay.Instance.IsRowCompleted(rowID))
        {
            int count = 0;
            foreach (Block block in GamePlay.Instance.GetEntireRow(rowID))
            {
                if (block.spriteType == SpriteType.MagnetWithBubble || block.spriteType == SpriteType.Magnet)
                {
                    count++;
                    if (count >= 2)
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            int count = 0;
            foreach (Block block in GamePlay.Instance.GetEntireRow(rowID))
            {
                if (block.spriteType == SpriteType.MagnetWithBubble || block.spriteType == SpriteType.Magnet)
                {
                    count++;
                }
            }
            if (count >= 2)
            {
                if (isBlockerStickEnabled)
                {
                    StartCoroutine(BlockerStickSpwaner.Instance.DestroyVerticalBlockerStick(rowID));
                }
                PlayRowBreakingEffectByMagnet(rowID);
                
            }
        }
        return false;
    }

    private bool CanColoumClear(int coloumID)
    {

        if (!GamePlay.Instance.IsColumnCompleted(coloumID))
        {
            int count = 0;
            foreach (Block block in GamePlay.Instance.GetEntirColumn(coloumID))
            {
                if (block.spriteType == SpriteType.MagnetWithBubble || block.spriteType == SpriteType.Magnet)
                {
                    count++;
                    if (count >= 2)
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            int count = 0;
            foreach (Block block in GamePlay.Instance.GetEntirColumn(coloumID))
            {
                if (block.spriteType == SpriteType.MagnetWithBubble || block.spriteType == SpriteType.Magnet)
                {
                    count++;
                }
            }
            if (count >= 2)
            {
                if (isBlockerStickEnabled)
                {
                    StartCoroutine(BlockerStickSpwaner.Instance.DestroyHorizontalBlockerStick(coloumID));
                }
                PlayColoumBreakingEffectByMagnet(coloumID);
            }
        }
        return false;
    }

    private void PlayRowBreakingEffectByMagnet(int rowId)
    {
        List<Block> blocks = GamePlay.Instance.GetEntireRow(rowId);
        InstantiateLightning(blocks[0], blocks[blocks.Count - 1]);
    }

    private void PlayColoumBreakingEffectByMagnet(int coloumId)
    {
        List<Block> blocks = GamePlay.Instance.GetEntirColumn(coloumId);
        InstantiateLightning(blocks[0], blocks[blocks.Count - 1]);
    }

    private void InstantiateLightning(Block startPosition, Block endPosition)
    {
        LightningBoltScript lightning = Instantiate<LightningBoltScript>(lightningBoltPrefab);
        lightning.StartObject = startPosition.gameObject;
        lightning.EndObject = endPosition.gameObject;

        Destroy(lightning.gameObject, 0.5f);
    }
}
