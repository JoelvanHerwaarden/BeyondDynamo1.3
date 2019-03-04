using System;
using Dynamo.Core;
using Dynamo.Extensions;

namespace BeyondDynamo.UI
{
    class SearchWorkspaceViewModel : NotificationObject, IDisposable
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
        public SearchWorkspaceViewModel(ReadyParams p)
        {
            readyParams = p;
        }
        public void Dispose()
        {
        }
    }
}
