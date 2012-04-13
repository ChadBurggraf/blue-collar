/**
 * Base extension of Backbone.Model for Blue Collar models.
 * Provides default attribute mappings, ASPNET date replacement
 * and URL behaviors.
 */
var BlueCollarModel = Backbone.Model.extend({
    initialize: function(attributes, options) {
        
        if (attributes) {
            // Backbone is not initializing attributes in initialize,
            // it is happening in the real object constructor. This is
            // hacky, but we need to replace the initialized attributes
            // with our replaced ones.
            this.set(BlueCollarModel.parseAllDates(attributes), {silent: true});
            this._changed = false;
            this._previousAttributes = _.clone(this.attributes);   
        }
    },

    isNew: function() {
        var id = this.get('Id');
        return _.isUndefined(id) || _.isNull(id) || _.isNaN(id) || id < 1;
    },

    parse: function(response) {
        return BlueCollarModel.parseAllDates(response);
    },

    set: function(attributes, options) {
        if (attributes && !_.isUndefined(attributes.Id)) {
            attributes.id = attributes.Id;
        }

        return Backbone.Model.prototype.set.call(this, attributes, options);
    },

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

    toJSON: function() {
        var obj = Backbone.Model.prototype.toJSON.call(this);
        delete obj.id;
        return obj;
    },

    url: function() {
        var baseUrl = this.collection && this.collection.url ? (_.isFunction(this.collection.url) ? this.collection.url() : this.collection.url) : '';
        baseUrl = baseUrl || this.urlRoot || urlError();
        return this.isNew() ? baseUrl : baseUrl.appendUrlPath(this.id);
    }
});

// BlueCollarModel static functions.
_.extend(BlueCollarModel, {
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