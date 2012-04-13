/**
 * Form view for ScheduledJob models.
 */
var ScheduledJobFormView = FormView.extend({
    serializers: {
        "Id": new IntFieldSerializer()
    },

    template: _.template($('#scheduled-job-form-template').html()),

    validators: {
        "JobType": [
            new RequiredValidator({message: 'Job type is required.'}),
            new LengthValidator({maxLength: 256, message: 'Job type cannot be longer than 256 characters.'})
        ]
    },

    initialize: function(options) {
        FormView.prototype.initialize.call(this, options);
        this.propertiesView = new PropertiesView({model: new Backbone.Model(JSON.parse(this.model.get('Properties') || '{}'))});
    },

    deserialize: function(attributes) {
        FormView.prototype.deserialize.call(this, attributes);
        this.propertiesView.model = new Backbone.Model(JSON.parse((attributes || {}).Properties || '{}'));
        return this;
    },

    focus: function() {
        this.$('input[name="JobType"]').focus();
    },

    render: function() {
        FormView.prototype.render.call(this);
        this.$('.input.properties').html(this.propertiesView.render().el);
        return this;
    },

    serialize: function() {
        var obj = FormView.prototype.serialize.call(this);
        obj['Properties'] = this.propertiesView.serialize();
        return obj;
    },

    validate: function(obj) {
        var errors = FormView.prototype.validate.call(this, obj),
            propertiesError;

        if (_.isUndefined(errors)) {
            propertiesError = this.propertiesView.validate(obj);

            if (propertiesError) {
                errors = {Properties: propertiesError};
            }
        }

        return errors;
    }
});