(function() {

  var log_methods = {'error': 5, 'warn': 4, 'info': 3, 'debug': 2, 'log': 1};

      var send_data = function(opts) {
        var img = document.createElement("img");
        img.src = opts.url + "?payload=" + encodeURIComponent(opts.data) + "&dt=" + encodeURIComponent(Date());
      }

  var castor = function(opts) {
    if (!opts.url) throw new Error("Please include a logphoria HTTP URL.");
    if (!opts.level) {
      this.level = log_methods['info'];
    } else {
      this.level = log_methods[opts.level.toLowerCase()];
    }
    var logger_factory = function(level_name) {
      return function(data) {
        if (log_methods[level_name] >= this.level) {
          if (typeof(data) != "string") {
            if (!data.type)
                data.type = level_name.toUpperCase();
            try {
              data = JSON.stringify(data);
            }
            catch (error) {}
          }
          opts.data = data;
          send_data(opts);
        }
      };
    };
    for (name in log_methods) {
      this[name] = logger_factory(name);
    }
  };

  if (this.logphoria) {
    this.logphoria.castor = castor;
  } else {
    this.logphoria = {castor: castor};
  }
})();
