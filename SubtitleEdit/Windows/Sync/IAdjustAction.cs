using System;

namespace Sync
{
    public interface IAdjustAction
    {
        void DoAdjustment(double milliseconds, AdjustmentSelection selection);
    }
}

