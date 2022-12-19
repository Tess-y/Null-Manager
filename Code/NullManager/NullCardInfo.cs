using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nullmanager{
public class NullCardInfo : CardInfo
{
    public CardInfo NulledSorce;
    public int PlayerId;

    internal bool randomCard = false;
}
}