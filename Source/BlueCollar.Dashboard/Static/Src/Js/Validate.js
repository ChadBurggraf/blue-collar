/**
 * Provides value validation services.
 *
 * @constructor
 * @this {Validator}
 * @param {object} options Additional initialization options.
 */
var Validator = function(options) {
    this.options = _.extend({}, options);
    this.initialize(options);
};

// Validator static functions.
_.extend(Validator, {
    extend: extend
});

// Validator prototype (instance functions).
_.extend(Validator.prototype, {
    initialize: function(options) {
        this.options = _.extend({
            message: 'Invalid.'
        }, options);
    },

    validate: function(value) {}
});

/**
 * Extends Validator to validate enumerations.
 */
var EnumValidator = Validator.extend({
    validate: function(value) {
        if (!_.isUndefined(value) && !_.isNull(value)) {
            value = value.toString();

            if (value.length > 0 && !_.any(this.options.possibleValues || [], function(o) { return o === value; })) {
                return this.options.message;
            }
        }
    }
});

/**
 * Extends Validator to validate string length.
 */
var LengthValidator = Validator.extend({
    validate: function(value) {
        if (!_.isUndefined(value) && !_.isNull(value)) {
            value = value.toString();

            if (value.length > this.options.maxLength) {
                return this.options.message;
            }
        }
    }
});

/**
 * Extends validator to validate number or date ranges.
 */
var RangeValidator = Validator.extend({
    validate: function(value) {
        if (!_.isUndefined(value) && !_.isNull(value)) {
            if (_.isNumber(value)) {
                if (value < this.options.min || value > this.options.max) {
                    return this.options.message;
                }
            } else if (_.isDate(value)) {
                if (!value.equals(this.options.min) && !value.equals(this.options.max) && !value.between(this.options.min, this.options.max)) {
                    return this.options.message;
                }
            }
        }
    }
});

/**
 * Extends Validator to validate against a regular expression.
 */
var RegexValidator = Validator.extend({
    validate: function(value) {
        if (!_.isUndefined(value) && !_.isNull(value) && _.isString(value) && value.length > 0) {
            if (_.isRegExp(this.options.exp)) {
                if (!this.options.exp.test(value)) {
                    return this.options.message;
                }
            } else {
                if (!(new RegExp(this.options.exp).test(value))) {
                    return this.options.message;
                }
            }
        }
    }
});

/**
 * Extends validator to validate required values.
 */
var RequiredValidator = Validator.extend({
    validate: function(value) {
        if (_.isUndefined(value)
            || _.isNull(value)
            || (_.isString(value) && value.length === 0)
            || (_.isNumber(value) && _.isNaN(value))
            || (_.isArray(value) && value.length === 0)
            || (jQuery.isPlainObject(value) && _.isEmpty(value))) {
            return this.options.message;
        }
    }
});