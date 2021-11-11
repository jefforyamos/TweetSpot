using System;

namespace TweetSpot.Delegates
{
    /// <summary>
    /// Used to designate the method that should be used to obtain the current UTC date and time.  Usually defaulted to <seealso cref="DateTime"/>.UtcNow.
    /// Overridden when doing unit testing.
    /// </summary>
    /// <returns>The method or property or expression used to determine the current date/time in UTC.</returns>
    public delegate DateTime UtcNowFunc();
}