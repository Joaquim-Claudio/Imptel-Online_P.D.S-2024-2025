export REDISCLI_AUTH=Imp7e1_db_session

if [ "$(redis-cli -h 127.0.0.1 -p 6379 DBSIZE)" -eq 0 ]; then

    redis-cli -h 127.0.0.1 -p 6379 "HMSET" "CURRENT_ACAD_YEAR" "absexp" "-1" "sldexp" "-1" "data" "2024/2025"
    redis-cli -h 127.0.0.1 -p 6379 "HMSET" "ACAD_YEAR_OPEN" "absexp" "-1" "sldexp" "-1" "data" "true"
    redis-cli -h 127.0.0.1 -p 6379 "HMSET" "NEXT_PAYMENT_SUFIX" "absexp" "-1" "sldexp" "-1" "data" "000012"
    redis-cli -h 127.0.0.1 -p 6379 "HMSET" "NEXT_STUDENT_SUFIX" "absexp" "-1" "sldexp" "-1" "data" "0002"
    redis-cli -h 127.0.0.1 -p 6379 "HMSET" "NEXT_TEACHER_SUFIX" "absexp" "-1" "sldexp" "-1" "data" "0001"
    redis-cli -h 127.0.0.1 -p 6379 "HMSET" "NEXT_SECRETARY_SUFIX" "absexp" "-1" "sldexp" "-1" "data" "0001"
    redis-cli -h 127.0.0.1 -p 6379 "HMSET" "NEXT_HELPDESK_SUFIX" "absexp" "-1" "sldexp" "-1" "data" "0001"
fi
