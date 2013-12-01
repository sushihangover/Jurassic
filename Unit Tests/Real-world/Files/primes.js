
				function listPrimes( nPrimes ) {
					var primes = [];
					for( var n = 2;  nPrimes > 0;  n++ ) {
						if( isPrime(n) ) {
							primes.push( n );
							--nPrimes;
						}
					}
					return primes;
				}

				function isPrime( n ) {
					var max = Math.sqrt(n);
					for( var i = 2;  i <= max;  i++ ) {
						if( n % i === 0 )
							return false;
					}
					return true;
				}


