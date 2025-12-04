using System.Collections.Generic;
using UnityEngine;

namespace mySdk
{
    [CreateAssetMenu(menuName = "Localisation", fileName = "LocalisationSO")]
    public class LocalisationDataSO : ScriptableObject
    {
        public List<LocalisationData> LocalisationList;
    }
}
