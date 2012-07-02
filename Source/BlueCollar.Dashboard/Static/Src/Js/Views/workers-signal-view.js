/**
 * Implements the workers signal form.
 *
 * @constructor
 * @extends {FormView}
 */
var WorkersSignalView = FormView.extend({
    serializers: {
        "Id": new IntFieldSerializer()
    },
    template: _.template($('#workers-signal-template').html()),
    validators: {
        "Id": [
            new RequiredFieldValidator({message: 'Id is required.'}),
            new RangeFieldValidator({min: 1, max: Number.MAX_VALUE, message: 'Id must be greater than 0.'})
        ],
        "Signal": [
            new RequiredFieldValidator({message: 'Signal is required.'}),
            new EnumFieldValidator({possibleValues: ['Start', 'Stop'], message: 'Signal must be either Start or Stop.'})
        ]
    },

    /**
     * Submits this form by serializing and validating the current inputs
     * If validation passes, the 'submit' event is raised. Otherwise, the
     * validation failure message(s) are rendered.
     *
     * @return {FormView} This instance.
     */
    submit: function() {
        var attributes = this.serialize(),
            errors = this.validate(attributes);

        this.renderErrors(errors);

        if (!errors) {
            this.trigger('submit', this, {Model: this.model, Attributes: attributes, Action: 'signalled'});
        }

        return this;
    },
});