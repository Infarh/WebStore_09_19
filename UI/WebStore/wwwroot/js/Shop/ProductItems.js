﻿ProductItems = {
	_properties: {
		getUrl: ""
	},

	init: properties => {
		$.extend(ProductItems._properties, properties);
		$(".pagination li a").click(ProductItems.clickOnPage);
	},

	clickOnPage: function (event) {
		event.preventDefault();
		const button = $(this);

		if (button.prop("href").length > 0) {
			var page = button.data("page");
			const container = $("#itemsContainer");
			container.LoadingOverlay("show");

			const data = button.data();

			let query = "";
			for (let key in data) {
				if (data.hasOwnProperty(key))
					query += `${key}=${data[key]}&`;
			}

			$.get(`${ProductItems._properties.getUrl}?${query}`)
				.done(result => {
					container.html(result);
					container.LoadingOverlay("hide");

					$(".pagination li").removeClass("active");
					$(".pagination li a").prop("href", "#");
					$(`.pagination li a[data-page=${page}]`)
						.removeAttr("href")
						.parent().addClass("active");
				})
				.fail(() => {
					console.log("clickOnPage getItems error");
				});
		}
	}
};