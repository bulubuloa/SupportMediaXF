using System;
using System.Collections.Generic;
using SupportMediaXF.Models;

namespace SupportMediaXF.Interfaces
{
    public interface ISupportMediaResultListener
    {
        void IF_PickedResult(List<SupportImageXF> result, int _CodeRequest);
    }
}