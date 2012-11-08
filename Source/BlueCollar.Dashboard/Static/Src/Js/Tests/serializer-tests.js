module('Serializers');

test('FieldSerializerBasicInputs', function() {
    var el = $('<form><input type="text" name="Name" value="Chad"/><textarea name="Description">Hello, world!</textarea><select name="Options"><option value="One"/><option value="Two" selected="selected"/></select></form>'),
        ser = new FieldSerializer();

    equal(ser.serialize(el.find('input')), 'Chad');
    equal(ser.serialize(el.find('textarea')), 'Hello, world!');
    equal(ser.serialize(el.find('select')), 'Two');
});

test('FieldSerializerBooleans', function() {
    var el = $('<form><input type="radio" name="Enabled" value="true"/><input type="radio" name="Enabled" value="false" checked="checked"/></form>'),
        ser = new BooleanFieldSerializer(),
        result;

    result = ser.serialize(el.find('input'));
    equal(false, result);
    
    ser.deserialize(false, el.find('input'));
    equal(el.find('input:checked').val(), 'false');
});

test('FieldSerializerCheckboxes', function() {
    var el = $('<form><input type="checkbox" name="State" value="Arizona" checked="checked"/><input type="checkbox" name="State" value="Colorado" checked="checked"/><input type="checkbox" name="State" value="California"/></form>'),
        ser = new FieldSerializer(),
        result;

    result = ser.serialize(el.find('input'));

    ok(_.isArray(result), 'Result is not an array.');
    equal(result.length, 2);
    equal(result[0], 'Arizona');
    equal(result[1], 'Colorado');
});

test('FieldSerializerDates', function() {
    var el = $('<form><input type="text" name="Date" value="2011-11-03 3:48 PM"/></form>'),
        ser = new DateFieldSerializer(),
        result;

    result = ser.serialize(el.find('input'));
    ok(_.isDate(result), 'Result is not a date.');
    equal(result.toString(), Date.parse('2011-11-03 3:48 PM').toString());

    ser.deserialize(Date.parse('2011-11-03 3:51 PM'), el.find('input'));
    equal(el.find('input').val(), '2011-11-03 3:51 PM');
});

test('FieldSerializerDoubles', function() {
    var el = $('<input type="text" name="Double" value="1.23"/>'),
        ser = new DoubleFieldSerializer(),
        result;

    result = ser.serialize(el);
    ok(_.isNumber(result), 'Result is not a number.');
    equal(1.23, result);

    ser.deserialize(4.567, el);
    equal(el.val(), '4.57');
});

test('FieldSerializerInts', function() {
    var el = $('<input type="text" name="Int" value="42"/>'),
        ser = new IntFieldSerializer(),
        result;

    result = ser.serialize(el);
    ok(_.isNumber(result), 'Result is not a number.');
    equal(42, result);

    ser.deserialize(12, el);
    equal(el.val(), '12');
});

test('FieldSerializerDeserializeBasicInputs', function() {
    var el = $('<form><input type="text" name="Name" value=""/><textarea name="Description"></textarea><select name="Options"><option value="One"/><option value="Two"/></select></form>'),
        ser = new FieldSerializer();

    ser.deserialize('Chad', el.find('input[name="Name"]'));
    equal(el.find('input[name="Name"]').val(), 'Chad');

    ser.deserialize('Hello, world!', el.find('textarea[name="Description"]'));
    equal(el.find('textarea[name="Description"]').val(), 'Hello, world!');

    ser.deserialize('Two', el.find('select[name="Options"]'));
    equal(el.find('select[name="Options"]').val(), 'Two');
});

test('FieldSerializerDeserializeCheckboxes', function() {
    var el = $('<form><input type="checkbox" name="State" value="Arizona" checked=""/><input type="checkbox" name="State" value="Colorado" checked=""/><input type="checkbox" name="State" value="California"/></form>'),
        ser = new FieldSerializer(),
        input,
        i,
        n;

    ser.deserialize(['Arizona', 'Colorado'], el.find('input[name="State"]'));
    
    for (i = 0, n = el.find('input[name="State"]').length; i < n; i++) {
        input = $(el.find('input[name="State"]')[i]);

        if (input.val() === 'Arizona' || input.val() === 'Colorado') {
            ok(input.is(':checked'), 'Arizona and Colorado should be checked.');
        } else {
            ok(!input.is(':checked'), input.val() + ' should not be checked.');
        }
    }
});

test('FormSerializerBasicForm', function() {
    var el = $('<form><input type="text" name="Name" value="Chad"/><textarea name="Description">Hello, world!</textarea><select name="Options"><option value="One"/><option value="Two" selected="selected"/></select><input type="checkbox" name="State" value="Arizona" checked="checked"/><input type="checkbox" name="State" value="Colorado" checked="checked"/><input type="checkbox" name="State" value="California"/></form>'),
        ser = new FormSerializer(),
        attributes = {
            Name: null,
            Description: null,
            Options: null,
            State: null
        },
        result;

    result = ser.serialize(el, attributes);
    ok(!_.isNull(result), 'Result is null.');
    ok(!_.isUndefined(result.Name), 'result.Name is undefined.');
    equal(result.Name, 'Chad');
    ok(!_.isUndefined(result.Description), 'result.Description is undefined.');
    equal(result.Description, 'Hello, world!');
    ok(!_.isUndefined(result.Options), 'result.Options is undefined.');
    equal(result.Options, 'Two');
    ok(!_.isUndefined(result.State), 'result.State is undefined.');
    ok(_.isArray(result.State), 'result.State is not an array');
    equal(result.State.length, 2);
    equal(result.State[0], 'Arizona');
    equal(result.State[1], 'Colorado');
});

test('FormSerializerDeserializeBasicForm', function() {
    var el = $('<form><input type="text" name="Name" value=""/><textarea name="Description"></textarea><select name="Options"><option value="One"/><option value="Two" selected=""/></select><input type="checkbox" name="State" value="Arizona" checked=""/><input type="checkbox" name="State" value="Colorado" checked=""/><input type="checkbox" name="State" value="California"/></form>'),
        ser = new FormSerializer(),
        attributes = {
            Name: 'Chad',
            Description: 'Hello, world!',
            Options: 'Two',
            State: ['California', 'Arizona']
        },
        input,
        i,
        n;

    ser.deserialize(el, attributes);
    equal(el.find('input[name="Name"]').val(), 'Chad');
    equal(el.find('textarea[name="Description"]').val(), 'Hello, world!');
    equal(el.find('select[name="Options"]').val(), 'Two');

    for (i = 0, n = el.find('input[name="State"]').length; i < n; i++) {
        input = $(el.find('input[name="State"]')[i]);

        if (input.val() === 'California' || input.val() === 'Arizona') {
            ok(input.is(':checked'), 'Arizona and California should be checked.');
        } else {
            ok(!input.is(':checked'), input.val() + ' should not be checked.');
        }
    }
});