Cart = {
	_properties: {
		addToCartLink: "",
		decrementLink: "",
		removeFromCartLink: "",
		removeAllLink: "",
		getCartViewLink: ""
	},

	init: function(properties) {
		$.extend(Cart._properties, properties);

		Cart.initEvents();
	},

	initEvents: function() {
		$("a.CallAddToCart").click(Cart.addToCart);
		$(".cart_quantity_up").click(Cart.incrementItem);
		$(".cart_quantity_down").click(Cart.decrementItem);
		$(".cart_quantity_delete").click(Cart.removeFromCart);
	},

	addToCart: function(event) {
		var button = $(this);

		event.preventDefault();

		var id = button.data("id");

		$.get(Cart._properties.addToCartLink + "/" + id)
			.done(function() {
				Cart.showToolTip(button);
				Cart.refreshCartView();
			})
			.fail(function () { console.log("addToCart error"); });
	},

	showToolTip : function(button) {
		button.tooltip({ title: "Добавлено в корзину" }).tooltip("show");
		setTimeout(function() {
			button.tooltip("destroy");
		}, 500);
	},

	refreshCartView: function() {
		var container = $("#cartContainer");
		$.get(Cart._properties.getCartViewLink)
			.done(function(result) { container.html(result); })
			.fail(function() { console.log("refreshCartView error"); });
	},

	incrementItem: function () { },

	decrementItem: function () { },

	removeFromCart: function () { }
};