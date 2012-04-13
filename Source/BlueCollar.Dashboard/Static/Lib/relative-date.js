/*            DO WHAT THE FUCK YOU WANT TO PUBLIC LICENSE
 *                    Version 2, December 2004
 *
 * Copyright (C) 2011 Azer Koculu <azer@kodfabrik.com>
 *
 * Everyone is permitted to copy and distribute verbatim or modified
 * copies of this license document, and changing it is allowed as long
 * as the name is changed.
 *
 *            DO WHAT THE FUCK YOU WANT TO PUBLIC LICENSE
 *   TERMS AND CONDITIONS FOR COPYING, DISTRIBUTION AND MODIFICATION
 *
 *  0. You just DO WHAT THE FUCK YOU WANT TO.
 */
(function(undefined) {
    var SECOND = 1000,
        MINUTE = 60 * SECOND,
        HOUR = 60 * MINUTE,
        DAY = 24 * HOUR,
        WEEK = 7 * DAY,
        YEAR = DAY * 365,
        MONTH = YEAR / 12,
        formats = [
            [ 2 * SECOND, 'a second ago' ],
            [ MINUTE, 'seconds ago', SECOND ],
            [ 1.5 * MINUTE, 'a minute ago' ],
            [ 60 * MINUTE, 'minutes ago', MINUTE ],
            [ 1.5 * HOUR, 'an hour ago' ],
            [ DAY, 'hours ago', HOUR ],
            [ 2 * DAY, 'yesterday' ],
            [ 7 * DAY, 'days ago', DAY ],
            [ 1.5 * WEEK, 'a week ago'],
            [ MONTH, 'weeks ago', WEEK ],
            [ 1.5 * MONTH, 'a month ago' ],
            [ YEAR, 'months ago', MONTH ],
            [ 1.5 * YEAR, 'a year ago' ],
            [ Number.MAX_VALUE, 'years ago', YEAR ]
        ];

    Date.prototype.toRelativeString = function(reference) {
        var time,
            delta,
            format, 
            i, 
            len;

        !reference && (reference = (new Date()).getTime());
        reference instanceof Date && (reference = reference.getTime());
        time = this.getTime();
        delta = reference - time;

        for (i = -1, len = formats.length; ++i < len;) {
            format = formats[i];

            if (delta < format[0]) {
                return format[2] === undefined ? format[1] : Math.round(delta / format[2]) + ' ' + format[1];
            }
        }
    };
})();