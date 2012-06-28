module('Validate');

test('ValidateRange', function() {
    equal(new RangeFieldValidator({min: 0, max: 1}).validate(1), undefined);
    equal(new RangeFieldValidator({min: 1, max: 2}).validate(1), undefined);
    equal(new RangeFieldValidator({min: 0, max: 2}).validate(0), undefined);
    notEqual(new RangeFieldValidator({min: 3, max: 4}).validate(2), undefined);
    equal(new RangeFieldValidator({min: 0, max: 100}).validate(null), undefined);
    equal(new RangeFieldValidator({min: Date.parse('2010-01-01'), max: Date.parse('2012-01-01')}).validate(Date.parse('2011-01-01')), undefined);
    notEqual(new RangeFieldValidator({min: Date.parse('2011-02-01'), max: Date.parse('2012-01-01')}).validate(Date.parse('2011-01-01')), undefined);
});

test('ValidateRegex', function() {
    equal(new RegexFieldValidator({exp: /^abc$/}).validate('abc'), undefined);
    equal(new RegexFieldValidator({exp: '^abc$'}).validate('abc'), undefined);
    notEqual(new RegexFieldValidator({exp: /^def$/}).validate('abc'), undefined);
    equal(new RegexFieldValidator({exp: /^abc$/}).validate(''), undefined);
});

test('ValidateRequired', function() {
    equal(new RequiredFieldValidator().validate('a'), undefined);
    notEqual(new RequiredFieldValidator().validate(''), undefined);
    equal(new RequiredFieldValidator().validate(0), undefined);
    equal(new RequiredFieldValidator().validate(12), undefined);
    notEqual(new RequiredFieldValidator().validate(NaN), undefined);
    equal(new RequiredFieldValidator().validate(new Date()), undefined);
    equal(new RequiredFieldValidator().validate([1, 2, 3]), undefined);
    notEqual(new RequiredFieldValidator().validate([]), undefined);
    notEqual(new RequiredFieldValidator().validate(undefined), undefined);
    notEqual(new RequiredFieldValidator().validate(null), undefined);
    notEqual(new RequiredFieldValidator().validate({}), undefined);
});