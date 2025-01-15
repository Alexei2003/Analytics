import { useEffect, useState } from 'react';
import './Survey.css';
import { useNavigate, useParams } from 'react-router-dom';
import { getUserRole, getEmployeeId } from './funcs/CookiesManager';
import { AddButtonListSurveysWithId, AddButtonUserProfile } from './funcs/Buttons';

function Survey() {
    const { id } = useParams();

    const [survey, setSurvey] = useState(null);

    const navigate = useNavigate();
    useEffect(() => {
        if (!getUserRole()) {
            navigate('/');
        }
        getSurvey(id);
    }, []);

    function AddImage(url) {
        if (!url) {
            return (
                <img src={"http://localhost/base-images/1232.webp"} />
            )
        }

        return (
            <img src={url} />
        )
    }

    function HeaderSurvey() {
        if (survey === null) {
            return null;
        }

        return (
            <div className="chart-container">
                <table className="table">
                    <tbody>
                        <tr>
                            <td>
                                <h3>{survey.title}</h3>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                {AddImage(survey.photoUrl)}
                            </td>
                            <td className="text-left">
                                <label>{survey.description}</label>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }

    const [answerArr, setAnswerArr] = useState();
    const handleChangeAnswer = (event) => {
        const answerArrLoc = answerArr;
        const { questionId, answerId } = JSON.parse(event.target.value);
        for (let i = 0; i < survey.questions.length; i++) {
            if (survey.questions[i].id === questionId) {
                answerArrLoc[i] = answerId;
                break;
            }
        }
        setAnswerArr(answerArrLoc);
    };
    function QuestionSurvey(question) {
        if (survey === null) {
            return null;
        }

        return (
            <>
                <div className="chart-container">
                    <div>
                        <label>{question.text}</label>
                    </div>
                    <select onChange={handleChangeAnswer}>
                        {question.answers.map((answer) => (
                            <option key={question.score} value={JSON.stringify({ questionId: question.id, answerId: answer.id })}>
                                {answer.text}
                            </option>
                        ))}
                    </select>
                </div>
            </>
        );
    }

    function ButtonsHeader() {
        return (
            <>
                <div className="chart-container">
                    <div className="profile-buttons">
                        {AddButtonListSurveysWithId(navigate, getEmployeeId())}
                        {AddButtonUserProfile(navigate)}
                    </div>
                </div>
            </>
        );
    }

    function ButtonEnd() {
        return (
            <>
                <div className="chart-container">
                    <div className="profile-buttons">
                        <button type="button" onClick={() => postAnswers()} className="list-surveys-btn animate-bounce"> Отправить ответы </button>
                    </div>
                </div>
            </>
        );
    }

    return (
        <>
            {ButtonsHeader()}
            {HeaderSurvey()}
            {survey && survey.questions.map((question) => (
                <>
                    {QuestionSurvey(question)}
                </>
            ))}
            {ButtonEnd()}
        </>
    );

    async function getSurvey(id) {
        try {
            const response = await fetch(`survey/${id}`, {
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                }
            });

            if (response.ok) {
                const data = await response.json();
                setSurvey(data);

                let array = [];

                for (let i = 0; i < data.questions.length; i++) {
                    array = [...array, data.questions[i].answers[0].id]
                }

                setAnswerArr(array);
            }

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('Ошибка при отправке:', error);
            navigate('/404');
        }
    }

    async function postAnswers() {
        try {
            const response = await fetch('survey', {
                method: 'POST',
                headers: {
                    "Content-type": "application/json",
                },
                body: JSON.stringify(answerArr),
            })

            if (response.ok) {
                navigate('/listsurveys');
            }

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }

        } catch (error) {
            console.error('Ошибка при отправке:', error);
        }

    }
}

export default Survey;