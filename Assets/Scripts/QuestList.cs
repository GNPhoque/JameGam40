using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObjects/QuestList")]
public class QuestList : ScriptableObject
{
	public List<Quest> quests;
}
