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