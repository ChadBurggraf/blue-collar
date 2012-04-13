/**
 * Provides form serialization services.
 *
 * @constructor
 * @this {FormSerializer}
 * @param {object} options Additional initialization options.
 */
var FormSerializer = function(options) { 
    this.options = _.extend({}, options);
    this.initialize(options);
};

// FormSerializer static functions.
_.extend(FormSerializer, {
    extend: extend,

    inputSelectors: function(prop) {
        return 'input[name="' + prop + '"], select[name="' + prop + '"], textarea[name="' + prop + '"]';
    },

    isJQuery: function(el) {
        return !_.isUndefined(el) && el instanceof jQuery && el.length > 0;
    }
});

// FormSerializer prototype (instance functions).
_.extend(FormSerializer.prototype, {
    initialize: function(options) { },

    deserialize: function(el, attributes, serializers) {
        var prop,
            fields,
            ser;

        attributes = attributes || {};
        serializers = serializers || {};

        if (FormSerializer.isJQuery(el)) {
            for (prop in attributes) {
                if (attributes.hasOwnProperty(prop)) {
                    fields = el.find(FormSerializer.inputSelectors(prop));
                    ser = serializers[prop] || new FieldSerializer();
                    ser.deserialize(attributes[prop], fields);
                }
            }
        }
    },

    serialize: function(el, attributes, serializers) {
        var prop,
            fields,
            ser,
            result = {};

        attributes = attributes || {};
        serializers = serializers || {};

        if (FormSerializer.isJQuery(el)) {
            for (prop in attributes) {
                if (attributes.hasOwnProperty(prop)) {
                    fields = el.find(FormSerializer.inputSelectors(prop));
                    ser = serializers[prop] || new FieldSerializer();
                    result[prop] = ser.serialize(fields);
                }
            }
        }

        return result;
    }
});

/**
 * Provides serialization services for individual fields in a form.
 *
 * @constructor
 * @this {FieldSerializer}
 * @param {object} options Additional initialization options.
 */
var FieldSerializer = function(options) {
    this.options = _.extend({}, options);
    this.initialize(this.options);
};

// FieldSerializer static functions.
_.extend(FieldSerializer, {
    extend: extend
});

// FieldSerializer prototype (instance functions).
_.extend(FieldSerializer.prototype, {
    initialize: function(options) { },

    deserialize: function(value, el) {
        var tagName,
            type,
            op,
            i,
            n;

        if (FormSerializer.isJQuery(el)) {
            tagName = (el[0].tagName || '').toUpperCase();
            type = (el.attr('type') || '').toUpperCase();

            if (tagName === 'INPUT' && type === 'CHECKBOX') {
                if (!_.isUndefined(value) && !_.isNull(value) && !_.isNaN(value)) {
                    if (!_.isArray(value)) {
                        value = [value.toString()];
                    }
                }

                value = (value || []).map(function(v) { return v.toString(); });

                for (i = 0, n = el.length; i < n; i++) {
                    el[i].checked = '';

                    if (_.any(value, function(v) { return v === $(el[i]).val(); })) {
                        el[i].checked = 'checked';
                    }
                }
            } else if (tagName === 'INPUT' && type === 'RADIO') {
                if (!_.isUndefined(value) && !_.isNull(value) && !_.isNaN(value)) {
                    value = value.toString();
                } else {
                    value = null;
                }

                for (i = 0, n = el.length; i < n; i++) {
                    el[i].checked = value === $(el[i]).val() ? 'checked' : '';
                }
            } else if (tagName === 'SELECT') {
                if (el[0].options.length > 0) {
                    if (!_.isUndefined(value) && !_.isNull(value) && !_.isNaN(value)) {
                        value = value.toString();
                        
                        for (i = el[0].options.length - 1; i >= 0; i--) {
                            op = el[0].options.item(i);
                            
                            if ((op.value && value === op.value) || value === op.text || i === 0) {
                                el[0].selectedIndex = i;
                                break;
                            }
                        }
                    } else {
                        el[0].selectedIndex = 0;
                    }
                }
            } else {
                if (!_.isUndefined(value) && !_.isNull(value) && !_.isNaN(value)) {
                    el.val(value.toString());
                } else {
                    el.val('');
                }
            } 
        }
    },

    serialize: function(el) {
        var tagName,
            type,
            i,
            n,
            val = null;

        if (FormSerializer.isJQuery(el)) {
            tagName = (el[0].tagName || '').toUpperCase();
            type = (el.attr('type') || '').toUpperCase();

            if (tagName === 'INPUT' && type === 'CHECKBOX') {
                val = [];

                for (i = 0, n = el.length; i < n; i++) {
                    if (el[i].checked) {
                        val.push($(el[i]).val());
                    }
                }
            } else if (tagName === 'INPUT' && type === 'RADIO') {
                for (i = 0, n = el.length; i < n; i++) {
                    if (el[i].checked) {
                        val = $(el[i]).val();
                        break;
                    }
                }
            } else {
                val = el.val();
            }
        }

        return val;
    }
});

/**
 * Extends FieldSerializer to serialize date/time inputs.
 */
var DateFieldSerializer = FieldSerializer.extend({
    initialize: function(options) {
        FieldSerializer.prototype.initialize.call(this, options);

        this.options = _.extend({
            format: 'yyyy-MM-dd h:mm tt'
        }, this.options);
    },

    deserialize: function(value, el) {
        if (FormSerializer.isJQuery(el)) {
            if (!_.isUndefined(value) && _.isDate(value)) {
                el.val(value.toString(this.options.format));
            } else {
                FieldSerializer.prototype.deserialize.call(this, value, el);
            }
        }
    },

    serialize: function(el) {
        if (FormSerializer.isJQuery(el)) {
            return Date.parse(el.val());
        }

        return null;
    }
});

/**
 * Extends FieldSerializer to serialize floating point numbers.
 */
var DoubleFieldSerializer = FieldSerializer.extend({
    initialize: function(options) {
        FieldSerializer.prototype.initialize.call(this, options);

        this.options = _.extend({
            digits: 2
        }, this.options);
    },

    deserialize: function(value, el) {
        if (FormSerializer.isJQuery(el)) {
            if (!_.isUndefined(value) && _.isNumber(value)) {
                el.val(new Number(value).toFixed(this.options.digits));
            } else {
                FieldSerializer.prototype.deserialize.call(this, value, el);
            }
        }
    },

    serialize: function(el) {
        var value;

        if (FormSerializer.isJQuery(el)) {
            value = el.val();
            
            if (jQuery.isNumeric(value)) {
                return parseFloat(value, 10);
            }       
        }

        return null;
    }
});

/**
 * Extends FieldSerializer to serialize integer numbers.
 */
var IntFieldSerializer = FieldSerializer.extend({
    deserialize: function(value, el) {
        if (FormSerializer.isJQuery(el)) {
            if (!_.isUndefined(value) && _.isNumber(value)) {
                el.val(Math.floor(value).toString());
            } else {
                FieldSerializer.prototype.deserialize.call(this, value, el);
            }
        }
    },

    serialize: function(el) {
        var value;

        if (FormSerializer.isJQuery(el)) {
            value = el.val();
            
            if (jQuery.isNumeric(value)) {
                return Math.floor(parseInt(value, 10));
            }       
        }

        return null;
    }
});

/**
 * Extends FieldSerializer to serialize QueueName strings.
 */
var QueueNamesSerializer = FieldSerializer.extend({
    deserialize: function(value, el) {
        value = $.splitAndTrim(value, '\n');

        if (_.any(value, function(s) { return s === '*'; })) {
            el.val('');
        } else {
            el.val(value.join('\n'));
        }
    },

    serialize: function(el) {
        var value = $.splitAndTrim(el.val(), '\n');
        
        if (_.any(value, function(s) { return s === '*'; })) {
            return '*';
        } else {
            return value.join('\n');
        }
    }
});