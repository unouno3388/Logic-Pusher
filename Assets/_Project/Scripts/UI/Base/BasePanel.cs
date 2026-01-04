using UnityEngine;

namespace UI.Base
{
    public class BasePanel : MonoBehaviour
    {
        protected bool isRemove = false;
        protected PanelType panel = PanelType.None;
        public virtual void OpenPanel(PanelType panel)
        {
            this.panel = panel;
            gameObject.SetActive(true);
        }
        public virtual void ClosePanel()
        {
            isRemove = true;            
            gameObject.SetActive(false);
            Destroy(gameObject);
            
            //Debug.Log(UIManager.Instance.)
            Debug.Log(name +" Panel dict key ¬O§_¦s¦b" + UIManager.Instance.panelDict.ContainsKey(panel));
            //Debug.Log(UIManager.Instance.panelDict[panel].name);
            if (UIManager.Instance.panelDict.ContainsKey(panel))
            {
                Debug.Log("Removing panel from UIManager: " + panel.ToString());
                UIManager.Instance.panelDict.Remove(panel);
            }

        }

    }
}
