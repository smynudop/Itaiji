using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itaiji.NetFramework;

/// <summary>
/// 
/// </summary>
public enum OperationStatus
{
    /// <summary>
    /// 
    /// </summary>
    Done = 0,
    /// <summary>
    /// 
    /// </summary>
    DestinationTooSmall = 1,
    /// <summary>
    /// 
    /// </summary>
    NeedMoreData = 2,
    /// <summary>
    /// 
    /// </summary>
    InvalidData = 3,
}
