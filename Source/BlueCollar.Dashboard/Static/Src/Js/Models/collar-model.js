/**
 * Base model implementation.
 *
 * @constructor
 */
var CollarModel = Backbone.Model.extend({
    /**
     * Initialization.
     *
     * @param {Object} app A set of initial model attribute values.
     * @param {Object} options Initialization options.
     */
    initialize: function(attributes, options) {
        options = options || {};
        this.fragment = options.fragment || '';
        
        if (attributes) {
            // Backbone is not initializing attributes in initialize,
            // it is happening in the real object constructor. This is
            // hacky, but we need to replace the initialized attributes
            // with our replaced ones.
            this.set(CollarModel.parseAllDates(attributes), {silent: true});
            this.changed = {};
            this._silent = {};
            this._pending = {};
            this._previousAttributes = _.clone(this.attributes);
        }
    },

    /**
     * Gets a value indicating whether this instance represents a new model.
     *
     * @return {boolean} True if the model is new, false otherwise.
     */
    isNew: function() {
        var id = this.get('Id');
        return _.isUndefined(id) || _.isNull(id) || _.isNaN(id) || id < 1;
    },

    /**
     * Parses the model's data as returned by the server.
     *
     * @param {Object} response The raw response object received from the server.
     * @return {Object} The parsed response object.
     */
    parse: function(response) {
        return CollarModel.parseAllDates(response);
    },

    /**
     * Parses the model's response.Data attribute as returned by the server.
     *
     * @param {Object} response The response to parse/ensure a Data attribute for.
     * @return {Object} The parsed response object.
     */
    parseData: function(response) {
        if (!response.Data) {
            response.Data = '{}';
        }

        if (_.isString(response.Data)) {
            try {
                response.Data = JSON.parse(response.Data);
            } catch (e) {
                response.Data = {};
            }
        }

        response.Data = JSON.stringify(response.Data, null, 2);
        return response;
    },

    /**
     * Sets the given attributes on this instance.
     *
     * @param {Object} attributes The attributes to set.
     * @param {Object} options The options to use when setting attributes.
     */
    set: function(attributes, options) {
        if (attributes && !_.isUndefined(attributes.Id)) {
            attributes.id = attributes.Id;
        }

        return Backbone.Model.prototype.set.call(this, attributes, options);
    },

    /**
     * Gets a copy of th emodel's attributes, suitable for editing in a UI.
     *
     * @return {Object} An editable copy of the model's underlying attributes.
     */
    toEditJSON: function() {
        var obj = this.toJSON(),
            prop;

        for (prop in obj) {
            if (obj.hasOwnProperty(prop)) {
                if (_.isUndefined(this.defaults[prop])) {
                    delete obj[prop];
                }
            }
        }

        return obj;
    },

    /**
     * Gets a copy of the model's attributes.
     *
     * @return {Object} A copy of the model's underlying attributes.
     */
    toJSON: function() {
        var obj = Backbone.Model.prototype.toJSON.call(this);
        delete obj.id;
        return obj;
    },

    /**
     * Gets the URL to use when interacting with the model on the server.
     *
     * @return {String} The model's server URL.
     */
    url: function() {
        var baseUrl = this.collection && this.collection.url ? (_.isFunction(this.collection.url) ? this.collection.url() : this.collection.url) : '';
        baseUrl = baseUrl || this.urlRoot || urlError();
        return this.isNew() ? baseUrl : baseUrl.appendUrlPath(this.id);
    }
});

/**
 * Static functions.
 */
_.extend(CollarModel, {
    /**
     * Searches the given attributes object for strings that look like
     * dates, and parses them into {Date} objects.
     *
     * @param {Object} attributes A hash of attributes to search.
     * @return {Object} The updated attributes object with all dates parsed.
     */
    parseAllDates: function(attributes) {
        var prop,
            value;

        if (attributes) {
            for (prop in attributes) {
                if (attributes.hasOwnProperty(prop)) {
                    value = attributes[prop];
                    
                    if (Date.isASPNET(value)) {
                        attributes[prop] = Date.parseASPNET(value);
                    } else if (Date.isISOString(value)) {
                        attributes[prop] = new Date(value);
                    }
                }
            }
        }

        return attributes;
    }
});