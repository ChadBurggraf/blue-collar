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