import { Oval } from 'react-loader-spinner';
import React, { useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faFrown } from '@fortawesome/free-solid-svg-icons';
import './weather.css';

function WeatherApp() {
    const [city, setCity] = useState('');
    const [country, setCountry] = useState('');
    const [weather, setWeather] = useState({
        loading: false,
        data: {},
        error: {},
        hasError: false,
    });

    const search = async (event) => {
        event.preventDefault();
        setWeather({ ...weather, loading: true });

        const url = `${process.env.REACT_APP_API_URL}?city=${encodeURIComponent(city)}&country=${encodeURIComponent(country)}`;

        try {
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'x-api-key': process.env.REACT_APP_API_KEY,
                },
            });

            if (response.status === 200) {
                const data = await response.json();
                const { description } = data;

                setWeather({
                    loading: false,
                    data: { description },
                    error: {},
                    hasError: false,
                });
            } else if (response.status === 401) {
                const errorMessage = await response.text();
                setWeather({
                    loading: false,
                    data: {},
                    error: { errorMessage: errorMessage || 'Unauthorized access (Invalid API Key)' },
                    hasError: true,
                });
            } else if (response.status === 429) {
                const errorMessage = await response.text();
                setWeather({
                    loading: false,
                    data: {},
                    error: { errorMessage: errorMessage || 'Rate limit exceeded, please try again later' },
                    hasError: true,
                });
            } else {
                const errorData = await response.json();
                const { errorMessage, description, statusCode } = errorData;

                setWeather({
                    loading: false,
                    data: {},
                    error: { errorMessage: errorMessage || 'Unexpected error occurred', description, statusCode },
                    hasError: true,
                });
            }
        } catch (error) {
            console.error("Error fetching weather data:", error);

            setWeather({
                loading: false,
                data: {},
                error: { errorMessage: 'Network error, please check your connection' },
                hasError: true,
            });
        }
    };

    return (
        <div className="WeatherCard">
            <h1 className="weather-title">
                Weather Information
            </h1>
            <div className="search">
                <input
                    required
                    type="text"
                    className="city-search"
                    placeholder="Enter City Name.."
                    name="city"
                    value={city}
                    onChange={(event) => setCity(event.target.value)}
                />
            </div>
            <br />
            <div className="search-bar">
                <input
                    required
                    type="text"
                    className="country-search"
                    placeholder="Enter Country Name.."
                    name="country"
                    value={country}
                    onChange={(event) => setCountry(event.target.value)}
                />
            </div>
            <br />
            <div className="search-button-div">
                <button className="search-button" onClick={search}>
                    Get Weather Information
                </button>
            </div>

            {weather.loading && (
                <>
                    <br />
                    <br />
                    <Oval type="Oval" color="black" height={100} width={100} />
                </>
            )}

            {weather.hasError && (
                <>
                    <br />
                    <br />
                    <span className="error-message">
                        <FontAwesomeIcon icon={faFrown} />
                        {weather.error.errorMessage && (
                            <span style={{ fontSize: '20px' }}>
                                {weather.error.errorMessage}
                            </span>
                        )}
                        {weather.error.description && (
                            <div>
                                <strong>Description:</strong> {weather.error.description}
                            </div>
                        )}

                    </span>
                </>
            )}

            {weather.data && weather.data.description && (
                <div>
                    <div className="description">
                        <p>{weather.data.description.toUpperCase()}</p>
                    </div>
                </div>
            )}
        </div>
    );
}

export default WeatherApp;
