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