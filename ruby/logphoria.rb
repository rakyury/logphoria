require 'net/http'
require 'net/https'
require 'rubygems'
require 'json'

class Logphoria
  attr_accessor :key
  def url=(url)
    @url = URI(url.to_s)
  end
  def url
    @url.to_s
  end

  def initialize(key, url='https://176.34.114.130/log/')
    @key = key
    self.url = url + @key
  end

  # Push anything to a logphoria
  def log(type, msg, meta=nil)
    post type, msg, meta
  end
  
  protected

  def post(type, msg, meta)
    request = build_request(type, msg, meta)
    connection = build_connection
    connection.start{|http| http.request request}
  end

  def build_request(type, msg, meta)
    request = Net::HTTP::Post.new @url.request_uri
    request['Accept'] = 'application/json'
    request['Content-Type'] = 'application/json'
    request.body = {
      'type' => type,
      'message' => msg,
      'metadata' => meta
    }.to_json
    request
  end

  def build_connection # NOTE: Does not open the connection
    connection = Net::HTTP.new @url.host, @url.port
    if @url.scheme == 'https'
      connection.use_ssl = true
      connection.verify_mode = OpenSSL::SSL::VERIFY_NONE
    end
    connection
  end
end

