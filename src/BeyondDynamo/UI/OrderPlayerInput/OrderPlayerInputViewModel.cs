using System;
using Dynamo.Core;
using Dynamo.Extensions;

namespace BeyondDynamo.UI
{
    class OrderPlayerInputViewModel : NotificationObject, IDisposable
    {
        private ReadyParams readyParams;
        public ReadyParams ReadyParamType
        {
            get
            {
                readyParams = getReadyParams();
                return readyParams;
            }
        }
        public ReadyParams getReadyParams()
        {
            return readyParams;
        }
        public OrderPlayerInputViewModel(ReadyParams p)
        {
            readyParams = p;
        }
        public void Dispose()
        {
        }
    }

    
}
