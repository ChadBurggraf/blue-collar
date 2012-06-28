/**
 * Provides field validation services.
 *
 * @constructor
 * @param {Object} options Initialization options.
 */
var FieldValidator = function(options) {
    this.options = _.extend({}, options);
    this.initialize(options);
};

/**
 * Static {FieldValidator} functions.
 */
_.extend(FieldValidator, {
    /**
     * Inheritence behavior mixin.
     */
    extend: extend
});

/**
 * Prototype {FieldValidator} functions.
 */
_.extend(FieldValidator.prototype, {
    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.options = _.extend({
            message: 'Invalid.'
        }, options);
    },

    /**
     * Executes validation against the given value.
     *
     * @param {Object} value The value to validate.
     * @return {String} The error message to display if validation failed, undefined otherwise.
     */
    validate: function(value) {}
});

/**
 * Extends {FieldValidator} to validate enumerations.
 *
 * @constructor
 * @extends {FieldValidator}
 */
var EnumFieldValidator = FieldValidator.extend({
    /**
     * Executes validation against the given value.
     *
     * @param {Object} value The value to validate.
     * @return {String} The error message to display if validation failed, undefined otherwise.
     */
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
 * Extends {FieldValidator} to validate string length.
 *
 * @constructor
 * @extends {FieldValidator}
 */
var LengthFieldValidator = FieldValidator.extend({
    /**
     * Executes validation against the given value.
     *
     * @param {Object} value The value to validate.
     * @return {String} The error message to display if validation failed, undefined otherwise.
     */
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
 * Extends {FieldValidator} to validate number or date ranges.
 *
 * @constructor
 * @extends {FieldValidator}
 */
var RangeFieldValidator = FieldValidator.extend({
    /**
     * Executes validation against the given value.
     *
     * @param {Object} value The value to validate.
     * @return {String} The error message to display if validation failed, undefined otherwise.
     */
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
 * Extends {FieldValidator} to validate against a regular expression.
 *
 * @constructor
 * @extends {FieldValidator}
 */
var RegexFieldValidator = FieldValidator.extend({
    /**
     * Executes validation against the given value.
     *
     * @param {Object} value The value to validate.
     * @return {String} The error message to display if validation failed, undefined otherwise.
     */
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
 * Extends {FieldValidator} to validate required values.
 *
 * @constructor
 * @extends {FieldValidator}
 */
var RequiredFieldValidator = FieldValidator.extend({
    /**
     * Executes validation against the given value.
     *
     * @param {Object} value The value to validate.
     * @return {String} The error message to display if validation failed, undefined otherwise.
     */
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