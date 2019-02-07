using System;
using Dynamo.Core;
using Dynamo.Extensions;

namespace BeyondDynamo.UI
{
    class TextBoxViewModel : NotificationObject, IDisposable
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
        public TextBoxViewModel(ReadyParams p)
        {
            readyParams = p;
        }
        public void Dispose()
        {
        }
    }
}
