using System;
using System.Collections.Generic;

namespace SupportMediaXF.Models
{
    public class SupportGalleryXF
    {
        public string Name { set; get; }

        public List<SupportImageXF> Images { set; get; }

        public SupportGalleryXF()
        {
            Images = new List<SupportImageXF>();
        }

        public string IF_GetTitle()
        {
            return Name;
        }

        public string IF_GetDescription()
        {
            return Name;
        }

        public string IF_GetIcon()
        {
            return Name;
        }

        public Action IF_GetAction()
        {
            return null;
        }

        public bool IF_GetChecked()
        {
            return false;
        }

        public void IF_SetChecked(bool _Checked)
        {

        }
    }
}