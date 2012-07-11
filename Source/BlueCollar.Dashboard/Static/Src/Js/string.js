/*
 * Static {String} functions.
 */
_.extend(String, {
    /**
     * Gets a display string for the given date value, with the
     * time portion wrapped in "light" markup.
     *
     * @param {Date} value The date to get the display string for.
     * @return A date display string with a light time portion.
     */
    dateDisplayLight: function(value) {
        var result = '';

        if (value) {
            result = value.toString('MMM d, yyyy')
                + ' <span class="light">'
                + value.toString('h:mm:ss tt')
                + '</span>';
        }

        return result;
    },

    /**
     * Gets a display string of HTML for the given exception XML.
     *
     * @param {String} ex An XML fragment describing an exception.
     * @return {String} A display exception string of HTML.
     */
    exceptionDisplay: function(ex) {
        var result = '',
            message,
            frames;

        if (ex) {
            ex = $($.parseXML(ex));
            message = ex.find('Message').text();
            frames = ex.find('Frame');

            if (message) {
                result += $('<p/>').text(message).outerHtml();
            }

            if (frames) {
                frames = _.map(frames, function(f) { return '<code>' + $(f).text() + '</code>'; }).join('\n');
                result += $('<div class="code"/>').html(frames).outerHtml();
            }
        }

        return result;
    },

    /**
     * Gets a display string representing a job history status.
     *
     * @param {String} value The status value to get a display string for.
     * @return A job history status display string.
     */
    historyStatusDisplay: function(value) {
        var str = (value || '').toUpperCase();

        switch (str) {
            case 'SUCCEEDED':
            case 'FAILED':
            case 'CANCELED':
            case 'INTERRUPTED':
                return value;
            case 'TIMEDOUT':
                return 'Timed Out';
            default:
                return 'None';
        }
    },

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
    },

    /**
     * Gets a display string for a schedule's repeat settings.
     *
     * @param {String} repeatType The schedule's repeat type.
     * @param {Number} repeatValue The schedule's repeat value.
     * @return {String} A display string for the schedule's repeat settings.
     */
    scheduleRepeatTypeDisplay: function(repeatType, repeatValue) {
        return repeatType && repeatType !== 'None'
            ? 'Every ' + repeatValue + ' ' + repeatType.toLowerCase()
            : 'No';
    },

    /**
     * Gets a display string for a signal.
     *
     * @param {String} signal The signal to get the display string for.
     * @return {String} An HTML display string for the signal.
     */
    signalDisplay: function(signal) {
        return signal === 'None'
            ? $('<a class="btn-signal" href="javascript:void(0);"/>').text(signal).outerHtml()
            : $.htmlEncode(signal);
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