using UnityEngine;

public class RecordsInteract : MonoBehaviour
{
    //assign in the inspector
    public RecordsMenu.RecordType recordType;
 
    private void OnMouseUp()
    {
        RecordsMenu.Instance.SelectRecord(recordType);
    }
}