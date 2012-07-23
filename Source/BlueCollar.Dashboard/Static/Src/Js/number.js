/**
 * Static {Number} functions and constants.
 */
_.extend(Number, {
    THOUSAND: 1000,
    MILLION: 1000000,
    BILLION: 1000000000
});

/**
 * Prototype {Number} functions.
 */
_.extend(Number.prototype, {
    /**
     * Formats this instance using the given format string.
     *
     * @param {String} format The format string to use.
     * @return {String} The formatted value.
     */
    format: function(format) {
        var hasComma,
            fnum,
            cnum,
            parr,
            j,
            m,
            n,
            i;

        if (!_.isString(format)) {
            return '';
        }

        hasComma = -1 < format.indexOf(','),
        psplit = format.replace(/[^0-9\.+-]/g, '').split('.'),
        that = this; 

        // compute precision
        if (1 < psplit.length) {
            // fix number precision
            that = that.toFixed(psplit[1].length);
        } else if (2 < psplit.length) {
            // error: too many periods
            throw('NumberFormatException: invalid format, formats should have no more than 1 period: ' + format);
        } else {
            // remove precision
            that = that.toFixed(0);
        } 

        // get the string now that precision is correct
        fnum = that.toString(); 

        // format has comma, then compute commas
        if (hasComma) {
            // remove precision for computation
            psplit = fnum.split('.'); 

            cnum = psplit[0];
            parr = [];
            j = cnum.length;
            m = Math.floor(j / 3);
            n = cnum.length % 3 || 3; // n cannot be ZERO or causes infinite loop 

            // break the number into chunks of 3 digits; first chunk may be less than 3
            for (i = 0; i < j; i += n) {
                if (i !== 0) {
                    n = 3;
                }
            
                parr[parr.length] = cnum.substr(i, n);
                m -= 1;
            } 

            // put chunks back together, separated by comma
            fnum = parr.join(','); 

            // add the precision back in
            if (psplit[1]) {
                fnum += '' + psplit[1];
            }
        } 

        // replace the number portion of the format with fnum
        return format.replace(/[\d,?\.?]+/, fnum);
    },

    /**
     * Gets this instance's value as an abbreviated string.
     * e.g., 14K instead of 14000.
     *
     * @param {String} tooBig The value to return if this instance is too big to be abbreviated.
     * @return {String} An abbreviated string.
     */
    toAbbreviatedString: function(tooBig) {
        var n;

        function format(num, abbr) {
            var div,
                dec,
                floor,
                str;

            if (num < 10) {
                num = Math.floor(num * 100);
                div = 100;
            } else if (num < 100) {
                num = Math.floor(num * 10);
                div = 10;
            } else {
                num = Math.floor(num);
                div = 1;
            }

            num = num / div;
            floor = Math.floor(num);
            dec = (num - floor) * 10;
            num = floor;
            str = num.toString();

            if (dec < 10 && (dec = Math.round(dec)) === 10) {
                dec = 9;
            }

            if (dec > 0) {
                str += '.' + dec;
            }

            return str + abbr;
        }

        if (isNaN(this)) {
            return '';
        } else if (this < Number.THOUSAND) {
            return this.toString();
        } else if (this < Number.MILLION) {
            return format(this / Number.THOUSAND, 'K');
        }  else if (this < Number.BILLION) {
            return format(this / Number.MILLION, 'M');
        } else {
            n = this / Number.BILLION;

            if (n < Number.THOUSAND) {
                return format(n, 'B');
            }
        }

        return _.isUndefined(tooBig) || !_.isString(tooBig) || tooBig.length < 1 ? 'Too many' : tooBig;
    }
});