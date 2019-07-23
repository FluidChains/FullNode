FROM microsoft/dotnet:2.1-sdk

RUN git clone https://github.com/exoeconomy/CivXFullNode --recursive --branch stats-1.0 \
   && cd /CivXFullNode/src/Stratis.StratisD \
	 dotnet build

VOLUME /root/.stratisnode

COPY Docker/Node.Stats/exos.conf.docker /root/.stratisnode/exos/EXOSMain/exos.conf

EXPOSE 4561 4562 37220

WORKDIR /CivXFullNode/src/Stratis.StratisD

CMD ["dotnet", "run"]
