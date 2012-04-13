/**
 * Form view for Queued models.
 */
var QueuedFormView = FormView.extend({
    serializers: {
        "Id": new IntFieldSerializer()
    },

    template: _.template($('#queued-form-template').html()),

    validators: {
        "JobType": [
            new RequiredValidator({message: 'Job type is required.'}),
            new LengthValidator({maxLength: 256, message: 'Job type cannot be longer than 256 characters.'})
        ],

        "QueueName": [
            new LengthValidator({maxLength: 24, message: 'Queue cannot be longer than 24 characters.'})
        ]
    },

    focus: function() {
        this.$('input[name="JobType"]').focus();
    },

    render: function() {
        FormView.prototype.render.call(this);
        this.$('textarea').tabby({tabString: '  '});
        return this;
    },

    validate: function(obj) {
        var errors = FormView.prototype.validate.call(this, obj);

        if (_.isUndefined(errors)) {
            obj.Data = $.trim(obj.Data || '{}');

            try {
                JSON.parse(obj.Data);
            } catch (e) {
                errors = {'Data': 'Data must be valid JSON, de-serializable into an instance of the specified job type.'};
            }
        }

        return errors;
    }
});