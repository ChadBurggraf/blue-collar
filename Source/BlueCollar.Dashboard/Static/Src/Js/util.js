/**
 * Static {Date} functions.
 */
_.extend(Date, {
    /**
     * Gets a value indicating whether the given value is a string
     * that looks like an ASP.NET date.
     *
     * @param {Object} value The value to check.
     * @return {boolean} True if the object is an ASP.NET date string, false otherwise.
     */
    isASPNET: function(value) {
        if (!_.isUndefined(value) && _.isString(value)) {
            return /^\/Date\([\d-+]+\)\/$/.test(value);
        }

        return false;
    },

    /**
     * Gets a value indicating whether the given value is a string
     * that looks like an ISO date.
     *
     * @param {Object} value The value to check.
     * @return {boolean} True if the value is an ISO date string, false otherwise.
     */
    isISOString: function(value) {
        if (!_.isUndefined(value) && _.isString(value)) {
            return /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?Z?$/.test(value);
        }

        return false;
    },

    /**
     * Parses an ASP.NET date string into a {Date}.
     *
     * @param {String} value The value to parse.
     * @return {Date} The parsed date.
     */
    parseASPNET: function(value) {
        var num;

        if (!_.isUndefined(value) && _.isString(value) && value.length > 0) {
            num = parseInt(value.toString().replace('/Date(', '').replace(')/', ''), 10);
        
            if (!_.isUndefined(num) && !isNaN(num)) {
                return new Date(num);
            }    
        }

        return null;
    }
});

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

/*
 * Static {String} functions.
 */
_.extend(String, {
    /**
     * Gets a display string identifying a Blue Collar machine.
     *
     * @param {String} name The machine name.
     * @param {String} address The machine address.
     * @param {String} beginSep The left separation token.
     * @param {String} endSep The right separation token.
     * @return {String} A machine display string.
     */
    machineDisplay: function(name, address, beginSep, endSep) {
        var result = '',
            beginSep = beginSep || '(',
            endSep = endSep || ')',
            sep = false;

        if (name) {
            result += $.htmlEncode(name);
        }

        if (address) {
            if (result) {
                result += ' ' + beginSep;
                sep = true;
            }

            result += $.htmlEncode(address);

            if (sep) {
                result += endSep;
            }
        }

        return result;
    },

    /**
     * Gets a machine display string using the 'light' style.
     *
     * @param {String} name The machine name.
     * @param {String} address The machine address.
     * @return {String} A machine display string.
     */
    machineDisplayLight: function(name, address) {
        return String.machineDisplay(name, address, '<span class="light">', '</span>');
    },

    /**
     * Gets a machine display string using the 'parens' style.
     *
     * @param {String} name The machine name.
     * @param {String} address The machine address.
     * @return {String} A machine display string.
     */
    machineDisplayParens: function(name, address) {
        return String.machineDisplay(name, address, '(', ')');
    },

    /**
     * Formats a newline-separated string of queue names for display.
     *
     * @param {String} value A newline-separated string of queue names.
     * @return {String} A queue names display string.
     */
    queueNamesDisplay: function(value) {
        value = $.splitAndTrim(value, '\n');
        return value.length === 0 || _.any(value, function(s) { return s === '*'; })
            ? '*'
            : value.join(', ');
    }
});

/**
 * Prototype {String} functions.
 */
_.extend(String.prototype, {
    /**
     * Appens a path part to this instance, treating it as a URL.
     *
     * @param {String} path The path part to append.
     * @return {String} This instance, updated.
     */
    appendUrlPath: function(path) {
        var parts,
            url,
            query;

        if (!_.isUndefined(path) && !_.isNull(path)) {
            path = path.toString();

            if (path.charAt[0] === '/') {
                path = path.substr(1);
            }

            if (path) {
                parts = this.split('?'),
                url = parts[0] + (parts[0].charAt(parts[0].length - 1) !== '/' ? '/' : ''),
                query = parts.length > 1 ? _.map(parts.slice(1), function(p) { return '?' + p; }).join('') : '';

                return url + encodeURIComponent(path) + query;
            }
        }

        return this;
    },

    /**
     * Gets a value indicating whether this instance is a valid JSON/JavaScript identifier.
     *
     * @return True if this instance is a valid identifier, false otherwise.
     */
    isValidIdentifier: function() {
        return /^[a-zA-Z_$][0-9a-zA-Z_$]*$/.test(this);
    },

    /**
     * Ensures this instance ends with a trailing slash.
     *
     * @return This instance with a trailing slash.
     */
    withTrailingSlash: function() {
        return this.lastIndexOf('/') !== this.length - 1
            ? this + '/'
            : this;
    }
});

/**
 * jQuery extensions.
 */
(function($) {
    /**
     * Gets the outer-HTML of this element.
     *
     * @return {String} This element's outer-HTML.
     */
    $.fn.outerHtml = function() {
        return this.wrap('<div/>').parent().html();
    };

    /**
     * HTML-encodes the given value.
     *
     * @param {Object} value The value to HTML-encode.
     * @return {String} The HTML-encoded value.
     */
    $.htmlEncode = function(value) {
        if (_.isUndefined(value) || _.isNull(value)) {
            value = '';
        }

        return $('<div/>').text(value).text();
    };

    /**
     * Splits the given value using the given separator, trimming each resulting value
     * and then removing any empty values.
     *
     * @param {String} value The value to split and trim.
     * @param {String} separator The token to split the string on.
     * @return {Array} The result array.
     */ 
    $.splitAndTrim = function(value, separator) {
        value = value || '';
        separator = separator || '';
        return _.filter(_.map(value.split(separator), function(s) { return $.trim(s || ''); }), function(s) { return s.length > 0; });
    };
})(jQuery);